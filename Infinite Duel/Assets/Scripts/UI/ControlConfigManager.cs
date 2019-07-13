using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using System;
using Sirenix.OdinInspector;

namespace Duel.UI
{
    public class ControlConfigManager : SerializedMonoBehaviour
    {
        [SerializeField]
        [LabelText("Binding UIs")]
        private BindingUI[] actionBindingUIs;

        private PlayerController[] playerControllerInfos = new PlayerController[2];

        private InputMapper inputMapper = new InputMapper();

        [SerializeField]
        private GameObject[] listeningPanels;

        public Color[] playerColors;

        private Coroutine controllerPolling;
        private bool rebinding;
        private static ControlConfigManager instance;

        public static ControlConfigManager Singleton
        {
            get
            {
                return instance ?? (instance = FindObjectOfType<ControlConfigManager>());
            }
        }

        public struct PlayerController
        {
            public Player player;
            public ControllerType selectedControllerType;
            public int selectedControllerId;

            public ControllerMap Map
            {
                get => player.controllers.maps.GetMap(Controller.type, Controller.id, GetCategory(player.id), Input.Constants.Layout.Keyboard.Default);
            }

            public Controller Controller
            {
                get => player.controllers.GetController(selectedControllerType, selectedControllerId);
            }

            public PlayerController(int id, ControllerType selectedControllerType, int selectedControllerId)
            {
                player = ReInput.players.GetPlayer(id);
                this.selectedControllerType = selectedControllerType;
                this.selectedControllerId = selectedControllerId;
            }
        }

        public string GetControllerElementForAction(int player, int actionId, AxisRange range, out int elementMapId)
        {
            //There is an element map for each part of an action, so Axis actions have two, As Such each must be iterated through
            foreach (var elementMap in playerControllerInfos[player].Map.ElementMapsWithAction(actionId, true))
            {
                //returns the first element which matches the axis range
                if (elementMap.ShowInField(range))
                {
                    elementMapId = elementMap.id;

                    return elementMap.elementIdentifierName;
                }
            }
            elementMapId = -1;
            return string.Empty;
        }

        private void OnEnable()
        {
            if (!ReInput.isReady)
                return;

            Load();

            foreach (BindingUI ui in actionBindingUIs)
            {
                ui.Initialize(this);
            }
            inputMapper.options.ignoreMouseXAxis = true;
            inputMapper.options.ignoreMouseYAxis = true;

            inputMapper.InputMappedEvent += OnInputMapped;
            inputMapper.ConflictFoundEvent += OnConflictFound;
            inputMapper.CanceledEvent += OnCancelMapping;

            for (int i = 0; i < 2; i++)
            {
                SetCurrentController(i, playerControllerInfos[i].Controller);
            }

            Refresh();
        }

        private void OnCancelMapping(InputMapper.CanceledEventData obj)
        {
            playerControllerInfos[obj.inputMapper.mappingContext.controllerMap.playerId].Map.enabled = true;
        }

        private void OnConflictFound(InputMapper.ConflictFoundEventData obj)
        {
            foreach (var item in obj.conflicts)
            {
                if (item.actionId == Input.Constants.Action.Accept)
                {
                    obj.responseCallback(InputMapper.ConflictResponse.Cancel);
                    return;
                }
            }
            obj.responseCallback(InputMapper.ConflictResponse.Replace);
        }

        private void OnDisable()
        {
            inputMapper.RemoveAllEventListeners();
            inputMapper.Stop();
        }

        private void OnStopped(int playerId, Action<InputMapper.StoppedEventData> stopCallback)
        {
            listeningPanels[playerId].SetActive(false);
            inputMapper.StoppedEvent -= stopCallback;
            rebinding = false;
        }

        private void OnInputMapped(InputMapper.InputMappedEventData obj)
        {
            playerControllerInfos[obj.actionElementMap.controllerMap.playerId].Map.enabled = true;

            Refresh();
            ReInput.userDataStore.Save();
        }

        public static int GetCategory(int player)
        {
            return player == Input.Constants.Player.Player0 ? Input.Constants.Category.Default : Input.Constants.Category.Player_Two;
        }

        public void StartRebinding(int playerId, int actionID, AxisRange axisRange, int elementMapId)
        {
            if (rebinding)
                return;
            rebinding = true;
            playerControllerInfos[playerId].Map.enabled = false;
            listeningPanels[playerId].SetActive(true);

            inputMapper.Stop();
            Action<InputMapper.StoppedEventData> stopDelegate = null;
            stopDelegate = (data) => OnStopped(playerId, stopDelegate);
            inputMapper.StoppedEvent += stopDelegate;

            StartCoroutine(DelayStartRebinding(playerId, actionID, axisRange, elementMapId));
        }

        private IEnumerator DelayStartRebinding(int playerId, int actionID, AxisRange axisRange, int elementMapId)
        {
            yield return new WaitForSeconds(.1f);
            inputMapper.Start(
                           new InputMapper.Context()
                           {
                               actionId = actionID,
                               controllerMap = playerControllerInfos[playerId].Map,
                               actionRange = axisRange,
                               actionElementMapToReplace = playerControllerInfos[playerId].Map.GetElementMap(elementMapId)
                           }
                       );
        }

        private void Refresh()
        {
            foreach (BindingUI ui in actionBindingUIs)
            {
                ui.Refresh();
            }
        }

        public void StartControllerSwap(int playerId)
        {
            if (controllerPolling != null)
            {
                StopCoroutine(controllerPolling);
            }
            controllerPolling = StartCoroutine(PollForController(playerId));
        }

        private IEnumerator PollForController(int playerId)
        {
            listeningPanels[playerId].SetActive(true);
            yield return null;

            var pollingInfo = ReInput.controllers.polling.PollAllControllersForFirstElementDown();

            while (!pollingInfo.success || pollingInfo.controllerType == ControllerType.Mouse)
            {
                yield return null;
                pollingInfo = ReInput.controllers.polling.PollAllControllersForFirstElementDown();
            }

            SetCurrentController(playerId, pollingInfo.controller);
            print($"{pollingInfo.controllerType} : {pollingInfo.controllerName}[{pollingInfo.controllerId}]");
        }

        private void SetCurrentController(int playerId, Controller controller)
        {
            playerControllerInfos[playerId].Map.enabled = false;

            playerControllerInfos[playerId].player.controllers.ClearAllControllers();

            playerControllerInfos[playerId].player.controllers.AddController(controller, false);

            playerControllerInfos[playerId].selectedControllerId = controller.id;
            playerControllerInfos[playerId].selectedControllerType = controller.type;
            playerControllerInfos[playerId].Map.enabled = true;

            listeningPanels[playerId].SetActive(false);
            Refresh();
        }

        public void Load(bool refresh = true)
        {
            ReInput.userDataStore.Load();

            for (int i = 0; i < playerControllerInfos.Length; i++)
            {
                for (int j = 0; j < ReInput.controllers.controllerCount; j++)
                {
                    Controller curController = ReInput.controllers.Controllers[j];
                    if (ReInput.controllers.IsControllerAssignedToPlayer(curController.type, curController.id, i))
                    {
                        ControllerType savedType = curController.type;
                        int savedId = curController.id;

                        playerControllerInfos[i] = new PlayerController(i, savedType, savedId);
                        break;
                    }
                }
            }
            if (refresh)
                Refresh();
        }
    }
}