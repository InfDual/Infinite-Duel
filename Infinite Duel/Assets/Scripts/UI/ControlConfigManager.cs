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

        [Rewired.PlayerIdProperty(typeof(Input.Constants.Player))]
        public int currentPlayerId;

        private PlayerController[] playerControllerInfos = {
            new PlayerController() { selectedControllerId = 0, selectedControllerType = ControllerType.Keyboard},
            new PlayerController() { selectedControllerId = 0, selectedControllerType = ControllerType.Keyboard}
        };

        private InputMapper inputMapper = new InputMapper();

        public event Action<int> SetCurrentPlayerEventHandler;

        [SerializeField]
        private GameObject listeningPanel;

        public Color selectedBindingColor;
        public Color inactivePlayerColor;

        private Coroutine controllerPolling;

        public struct PlayerController
        {
            public ControllerType selectedControllerType;
            public int selectedControllerId;
        }

        public Player CurrentPlayer
        {
            get => ReInput.players.GetPlayer(currentPlayerId);
        }

        public ControllerMap CurrentControllerMap
        {
            get => CurrentPlayer.controllers.maps.GetMap(CurrentController.type, CurrentController.id, Input.Constants.Category.Default, Input.Constants.Layout.Keyboard.Default);
        }

        private Controller CurrentController
        {
            get => CurrentPlayer.controllers.GetController(playerControllerInfos[currentPlayerId].selectedControllerType, playerControllerInfos[currentPlayerId].selectedControllerId);
        }

        public void SetCurrentPlayer(int playerID)
        {
            currentPlayerId = playerID;
            SetCurrentPlayerEventHandler?.Invoke(playerID);
            Refresh();
        }

        public string GetControllerElementForAction(int actionId, AxisRange range, out int elementMapId)
        {
            //There is an element map for each part of an action, so Axis actions have two, As Such each must be iterated through
            foreach (var elementMap in CurrentPlayer.controllers.maps.ElementMapsWithAction(actionId, true))
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
            foreach (BindingUI ui in actionBindingUIs)
            {
                ui.Initialize(this);
            }

            if (!ReInput.isReady)
                return;

            Load();
            SetCurrentPlayer(Input.Constants.Player.Player0);
            inputMapper.options.ignoreMouseXAxis = true;
            inputMapper.options.ignoreMouseYAxis = true;

            inputMapper.InputMappedEvent += OnInputMapped;
            inputMapper.StoppedEvent += OnStopped;

            SetCurrentController(CurrentController);

            Refresh();
        }

        private void OnDisable()
        {
            inputMapper.RemoveAllEventListeners();
            inputMapper.Stop();
        }

        private void OnStopped(InputMapper.StoppedEventData obj)
        {
            CurrentPlayer.controllers.maps.SetMapsEnabled(true, Input.Constants.Category.UI);
            listeningPanel.SetActive(false);
        }

        private void OnInputMapped(InputMapper.InputMappedEventData obj)
        {
            Refresh();
            ReInput.userDataStore.Save();
        }

        public void StartRebinding(int actionID, AxisRange axisRange, int elementMapId)
        {
            CurrentPlayer.controllers.maps.SetMapsEnabled(false, Input.Constants.Category.UI);
            listeningPanel.SetActive(true);
            inputMapper.Start(
               new InputMapper.Context()
               {
                   actionId = actionID,
                   controllerMap = CurrentControllerMap,
                   actionRange = axisRange,
                   actionElementMapToReplace = CurrentControllerMap.GetElementMap(elementMapId)
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

        public void StartControllerSwap()
        {
            controllerPolling = StartCoroutine(PollForController());
        }

        private IEnumerator PollForController()
        {
            listeningPanel.SetActive(true);
            yield return null;

            var pollingInfo = ReInput.controllers.polling.PollAllControllersForFirstElementDown();

            while (!pollingInfo.success)
            {
                yield return null;
                pollingInfo = ReInput.controllers.polling.PollAllControllersForFirstElementDown();
            }

            SetCurrentController(pollingInfo.controller);
            print($"{pollingInfo.controllerType} : {pollingInfo.controllerName}[{pollingInfo.controllerId}]");
        }

        private void SetCurrentController(Controller controller)
        {
            CurrentPlayer.controllers.maps.SetMapsEnabled(false, CurrentController.type, Input.Constants.Category.Default);
            CurrentPlayer.controllers.ClearAllControllers();

            playerControllerInfos[currentPlayerId].selectedControllerType = controller.type;
            playerControllerInfos[currentPlayerId].selectedControllerId = controller.id;

            CurrentPlayer.controllers.AddController(controller, false);
            CurrentPlayer.controllers.maps.SetMapsEnabled(true, CurrentController.type, Input.Constants.Category.Default);

            listeningPanel.SetActive(false);
            Refresh();
        }

        public void Load()
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
                        int savedId = ReInput.players.GetPlayer(i).controllers.GetController(savedType, 0).id;

                        playerControllerInfos[i] = new PlayerController()
                        {
                            selectedControllerType = curController.type,
                            selectedControllerId = curController.id
                        };
                    }
                }
            }

            Refresh();
        }
    }
}