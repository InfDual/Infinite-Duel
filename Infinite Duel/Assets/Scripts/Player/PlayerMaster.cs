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
            if (module is IJumpUpdateSubscriber)
                JumpEventHandler += ((IJumpUpdateSubscriber)module).OnJump;

            if (module is IInputUpdateSubscriber)
                InputUpdateEventHandler += ((IInputUpdateSubscriber)module).OnInputUpdate;

            if (module is IGroundStateSubscriber)
                GroundStateUpdateEventHandler += ((IGroundStateSubscriber)module).OnGroundStateUpdate;

            if (module is IAttackSubscriber)
                AttackEventHandler += ((IAttackSubscriber)module).OnAttack;

            if (module is IDirectionFacingUpdateSubscriber)
                DirectionFacingUpdateEventHandler += ((IDirectionFacingUpdateSubscriber)module).OnDirectionFacingUpdate;
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
            }
        }
    }
}