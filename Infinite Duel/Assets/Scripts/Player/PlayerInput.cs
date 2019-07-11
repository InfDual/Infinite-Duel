using System;
using System.Collections;
using System.Collections.Generic;
using Rewired;
using UnityEngine;

namespace Duel.PlayerSystems
{
    /// <summary>
    /// Handles the input buffer
    /// </summary>
    public class PlayerInput : PlayerModule
    {
        private Rewired.Player input;

        protected override void OnInitialize()
        {
            input = Rewired.ReInput.players.GetPlayer(master.playerID);
            input.AddInputEventDelegate(
                OnInputDown,
                Rewired.UpdateLoopType.Update,
                Rewired.InputActionEventType.ButtonJustPressed);

            input.AddInputEventDelegate(
                OnAxisUpdate,
                Rewired.UpdateLoopType.Update,
                Rewired.InputActionEventType.AxisActiveOrJustInactive);

            input.AddInputEventDelegate(
                  OnAxisUpdate,
                  Rewired.UpdateLoopType.Update,
                  Rewired.InputActionEventType.AxisRawActiveOrJustInactive);
        }

        private void OnAxisUpdate(InputActionEventData obj)
        {
            PlayerInputUpdateEvent inputUpdateEvent = new PlayerInputUpdateEvent(PlayerInputButton.None, ButtonPhase.NotPressed, input);
            master.InvokePlayerEvent(inputUpdateEvent);
        }

        private void OnInputDown(InputActionEventData obj)
        {
            if (obj.eventType == InputActionEventType.ButtonJustPressed)
            {
                PlayerInputUpdateEvent inputUpdateEvent = new PlayerInputUpdateEvent(PlayerInputButton.None, ButtonPhase.Down, input);
                switch (obj.actionId)
                {
                    case Input.Constants.Action.Normal:
                        inputUpdateEvent.button = PlayerInputButton.Normal;
                        break;

                    case Input.Constants.Action.Special:
                        inputUpdateEvent.button = PlayerInputButton.Special;
                        break;

                    case Input.Constants.Action.Jump:
                        inputUpdateEvent.button = PlayerInputButton.Jump;
                        break;

                    case Input.Constants.Action.Block:
                        inputUpdateEvent.button = PlayerInputButton.Block;
                        break;

                    case Input.Constants.Action.Throw:
                        inputUpdateEvent.button = PlayerInputButton.Throw;
                        break;

                    case Input.Constants.Action.Pause:
                        inputUpdateEvent.button = PlayerInputButton.Pause;
                        break;
                }

                master.InvokePlayerEvent(inputUpdateEvent);
            }
        }
    }
}