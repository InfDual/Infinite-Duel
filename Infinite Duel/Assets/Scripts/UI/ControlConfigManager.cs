using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using System;

namespace Duel.Input
{
    public class ControlConfigManager : MonoBehaviour
    {
        [Rewired.PlayerIdProperty(typeof(Constants.Player))]
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
            get => CurrentPlayer.controllers.maps.GetMap(CurrentController.type, CurrentController.id, Constants.Category.Default, Constants.Layout.Keyboard.Default);
        }

        private Controller CurrentController
        {
            get => CurrentPlayer.controllers.GetController(selectedControllerType, selectedControllerId);
        }

        public string GetControllerElementNameForAction(int actionId)
        {
            return CurrentPlayer.controllers.maps.GetFirstElementMapWithAction(actionId, true).elementIdentifierName;
        }

        private void OnEnable()
        {
            if (!ReInput.isReady)
                return;
            Debug.Log(CurrentPlayer.controllers.maps.GetFirstElementMapWithAction(Input.Constants.Action.Jump, true).elementIdentifierName);

            inputMapper.options.ignoreMouseXAxis = true;
            inputMapper.options.ignoreMouseYAxis = true;

            ReInput.ControllerConnectedEvent += OnControllerChanged;
            ReInput.ControllerDisconnectedEvent += OnControllerChanged;
            inputMapper.InputMappedEvent += OnInputMapped;
            inputMapper.StoppedEvent += OnStopped;
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
        }

        private void OnInputMapped(InputMapper.InputMappedEventData obj)
        {
        }

        private void OnControllerChanged(ControllerStatusChangedEventArgs obj)
        {
            print(obj.name);
        }
    }
}