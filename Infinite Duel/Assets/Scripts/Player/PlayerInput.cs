using System;
using System.Collections;
using System.Collections.Generic;
using Rewired;
using UnityEngine;
using Actions = Duel.Input.Constants.Action;
using Sirenix.OdinInspector;

namespace Duel.PlayerSystems
{
    /// <summary>
    /// Handles the input buffer
    /// </summary>
    public class PlayerInput : PlayerModule, IDirectionFacingUpdateSubscriber
    {
        private Rewired.Player input;

        [OnValueChanged("UpdateBufferSize")]
        [SerializeField]
        private int bufferLength;

        [SerializeField]
        private int ticksPerSeconds;

        [ShowInInspector]
        private List<PlayerInputButton> inputBuffer = new List<PlayerInputButton>();

        private Coroutine bufferCoroutine;
        private bool inputRegistered;
        private bool facingRight;

        protected override void OnInitialize()
        {
            for (int i = 0; i < bufferLength; i++)
            {
                inputBuffer.Add(PlayerInputButton.None);
            }

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

            bufferCoroutine = StartCoroutine(UpdateBuffer());
        }

        private void OnDisable()
        {
            input?.RemoveInputEventDelegate(OnInputDown);
            input?.RemoveInputEventDelegate(OnAxisUpdate);
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
                    case Actions.Normal:
                        inputUpdateEvent.button = PlayerInputButton.Normal;
                        break;

                    case Actions.Special:
                        inputUpdateEvent.button = PlayerInputButton.Special;
                        break;

                    case Actions.Jump:
                        inputUpdateEvent.button = PlayerInputButton.Jump;
                        break;

                    case Actions.Block:
                        inputUpdateEvent.button = PlayerInputButton.Block;
                        break;

                    case Actions.Throw:
                        inputUpdateEvent.button = PlayerInputButton.Throw;
                        break;

                    case Actions.Pause:
                        inputUpdateEvent.button = PlayerInputButton.Pause;
                        break;

                    case Actions.Vertical:
                        inputUpdateEvent.button = obj.GetAxis() > 0 ? PlayerInputButton.Up : PlayerInputButton.Down;
                        break;

                    case Actions.Horizontal:
                        inputUpdateEvent.button = obj.GetAxis() > 0 ? PlayerInputButton.Right : PlayerInputButton.Left;
                        break;
                }
                if (inputUpdateEvent.button != PlayerInputButton.None && inputUpdateEvent.button != PlayerInputButton.Pause && !inputRegistered)
                {
                    inputBuffer.Insert(0, inputUpdateEvent.button);
                    inputRegistered = true;
                }

                master.InvokePlayerEvent(inputUpdateEvent);
            }
        }

        private IEnumerator UpdateBuffer()
        {
            while (true)
            {
                if (!inputRegistered)
                {
                    inputBuffer.Insert(0, GetCurrentAction());
                }
                inputRegistered = false;
                HandleAction(inputBuffer[bufferLength]);
                inputBuffer.RemoveAt(bufferLength);
                yield return new WaitForSeconds(1f / ticksPerSeconds);
            }
        }

        private void HandleAction(PlayerInputButton playerInputButton)
        {
            if (playerInputButton == PlayerInputButton.Normal || playerInputButton == PlayerInputButton.Special)
            {
                AttackType type = playerInputButton == PlayerInputButton.Normal ? AttackType.Normal : AttackType.Special;
                AttackDirection nextDirection = AttackDirection.Neutral;
                for (int i = bufferLength - 1; i >= 0; i--)
                {
                    switch (inputBuffer[i])
                    {
                        case PlayerInputButton.Up:
                            nextDirection = AttackDirection.Up;
                            break;

                        case PlayerInputButton.Right:
                            nextDirection = facingRight ? AttackDirection.Forwards : AttackDirection.Backwards;
                            break;

                        case PlayerInputButton.Down:
                            nextDirection = AttackDirection.Down;
                            break;

                        case PlayerInputButton.Left:
                            nextDirection = facingRight ? AttackDirection.Backwards : AttackDirection.Forwards;
                            break;
                    }
                    if (nextDirection != AttackDirection.Neutral)
                        break;
                }

                master.InvokePlayerEvent(new PlayerAttackEvent(type, nextDirection));
            }
        }

        private PlayerInputButton GetCurrentAction()
        {
            PlayerInputButton currentAction = PlayerInputButton.None;

            if (input.GetButtonDown(Actions.Normal))
            {
                currentAction = PlayerInputButton.Normal;
            }
            else if (input.GetButtonDown(Actions.Special))
                currentAction = PlayerInputButton.Special;
            else if (input.GetButtonDown(Actions.Block))
                currentAction = PlayerInputButton.Block;
            else if (input.GetButtonDown(Actions.Throw))
                currentAction = PlayerInputButton.Throw;
            else if (input.GetButtonDown(Actions.Jump))
                currentAction = PlayerInputButton.Jump;
            else if (input.GetAxis(Actions.Horizontal) > 0)
                currentAction = PlayerInputButton.Right;
            else if (input.GetAxis(Actions.Horizontal) < 0)
                currentAction = PlayerInputButton.Left;
            else if (input.GetAxis(Actions.Vertical) > 0)
                currentAction = PlayerInputButton.Up;
            else if (input.GetAxis(Actions.Vertical) < 0)
                currentAction = PlayerInputButton.Down;

            return currentAction;
        }

        private void UpdateBufferSize()
        {
            if (inputBuffer == null)
                return;
            inputBuffer.Clear();
            for (int i = 0; i < bufferLength; i++)
            {
                inputBuffer.Add(PlayerInputButton.None);
            }
        }

        public void OnDirectionFacingUpdate(PlayerDirectionFacingUpdateEvent eventArgs)
        {
            facingRight = eventArgs.FacingForwards;
        }
    }
}