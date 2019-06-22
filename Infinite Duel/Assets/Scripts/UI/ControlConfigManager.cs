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

        public ControllerType selectedControllerType;
        public int selectedControllerId;

        private InputMapper inputMapper = new InputMapper();

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
            get => CurrentPlayer.controllers.GetController(selectedControllerType, selectedControllerId);
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

            inputMapper.options.ignoreMouseXAxis = true;
            inputMapper.options.ignoreMouseYAxis = true;

            ReInput.ControllerConnectedEvent += OnControllerChanged;
            ReInput.ControllerDisconnectedEvent += OnControllerChanged;
            inputMapper.InputMappedEvent += OnInputMapped;
            inputMapper.StoppedEvent += OnStopped;

            Refresh();
        }

        private void OnDisable()
        {
            inputMapper.RemoveAllEventListeners();
            inputMapper.Stop();
            ReInput.ControllerConnectedEvent -= OnControllerChanged;
            ReInput.ControllerDisconnectedEvent -= OnControllerChanged;
        }

        private void OnStopped(InputMapper.StoppedEventData obj)
        {
            CurrentPlayer.controllers.maps.SetMapsEnabled(true, Input.Constants.Category.UI);
        }

        private void OnInputMapped(InputMapper.InputMappedEventData obj)
        {
            Refresh();
        }

        private void OnControllerChanged(ControllerStatusChangedEventArgs obj)
        {
            print(obj.name);
        }

        public void StartRebinding(int actionID, AxisRange axisRange, int elementMapId)
        {
            CurrentPlayer.controllers.maps.SetMapsEnabled(false, Input.Constants.Category.UI);

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
    }
}