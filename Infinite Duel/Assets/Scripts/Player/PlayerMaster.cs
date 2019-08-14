using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using System;
using System.Linq;
using System.Reflection;
using Rewired;

namespace Duel.PlayerSystems
{
    public class PlayerMaster : SerializedMonoBehaviour
    {
        [LabelText("Player ID")]
        public int playerID;

        #region Subscribers

        private event InputUpdateSubscription InputUpdateEventHandler;

        private event GroundStateUpdateSubscription GroundStateUpdateEventHandler;

        private event JumpSubscription JumpEventHandler;

        private event AttackSubscription AttackEventHandler;

        private event DirectionFacingUpdateSubscription DirectionFacingUpdateEventHandler;

        private event AnimationStateEventSubscription AnimationStateEventHandler;

        private event AnimationEventSubscription AnimationEventHandler;

        //Refers to event which occurs when a hitbox enters this player's hurtbox
        private event HitSubscription HitEventHandler;

        #endregion Subscribers

        private List<PlayerModule> linkedModules = new List<PlayerModule>();

        private void Awake()
        {
            InitializePlayerModules();
        }

        private void InitializePlayerModules()
        {
            linkedModules.AddRange(GetComponentsInChildren<PlayerModule>());

            foreach (PlayerModule module in linkedModules)
            {
                module.Initialize(this);
                SubscribeModuleToEvents(module);
            }
        }

        private void SubscribeModuleToEvents(PlayerModule module)
        {
            if (module is IJumpUpdateSubscriber jumpUpdateSubscriber)
                JumpEventHandler += jumpUpdateSubscriber.OnJump;

            if (module is IInputUpdateSubscriber inputUpdateSubscriber)
                InputUpdateEventHandler += inputUpdateSubscriber.OnInputUpdate;

            if (module is IGroundStateSubscriber groundStateSubscriber)
                GroundStateUpdateEventHandler += groundStateSubscriber.OnGroundStateUpdate;

            if (module is IAttackSubscriber attackSubscriber)
                AttackEventHandler += attackSubscriber.OnAttack;

            if (module is IDirectionFacingUpdateSubscriber directionFacingUpdateSubscriber)
                DirectionFacingUpdateEventHandler += directionFacingUpdateSubscriber.OnDirectionFacingUpdate;

            if (module is IAnimationStateEventSubscriber animationStateEventSubscriber)
                AnimationStateEventHandler += animationStateEventSubscriber.OnAnimationStateEvent;

            if (module is IAnimationEventSubscriber animationEventSubscriber)
                AnimationEventHandler += animationEventSubscriber.OnAnimationEvent;

            if (module is IHitSubscriber hitSubscriber)
                HitEventHandler += hitSubscriber.OnHit;
        }

        public void InvokePlayerEvent(IPlayerEvent playerEvent)
        {
            switch (playerEvent.EventType)
            {
                case PlayerEventType.Input:
                    InputUpdateEventHandler?.Invoke((PlayerInputUpdateEvent)playerEvent);
                    break;

                case PlayerEventType.Jump:
                    JumpEventHandler?.Invoke((PlayerJumpEvent)playerEvent);
                    break;

                case PlayerEventType.GroundStateUpdate:
                    GroundStateUpdateEventHandler?.Invoke((PlayerGroundStateUpdateEvent)playerEvent);
                    break;

                case PlayerEventType.Attack:
                    AttackEventHandler?.Invoke((PlayerAttackEvent)playerEvent);
                    break;

                case PlayerEventType.DirectionFacingUpdate:
                    DirectionFacingUpdateEventHandler?.Invoke((PlayerDirectionFacingUpdateEvent)playerEvent);
                    break;

                case PlayerEventType.AnimationState:
                    AnimationStateEventHandler?.Invoke((PlayerAnimationStateEvent)playerEvent);
                    break;

                case PlayerEventType.AnimationEvent:
                    AnimationEventHandler?.Invoke((PlayerAnimationEvent)playerEvent);
                    break;

                case PlayerEventType.Hit:
                    HitEventHandler?.Invoke((PlayerHitEvent)playerEvent);
                    break;
            }
        }
    }
}