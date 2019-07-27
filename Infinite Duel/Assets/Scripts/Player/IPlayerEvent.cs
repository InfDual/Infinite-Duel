using UnityEngine;

namespace Duel.PlayerSystems
{
    public interface IPlayerEvent
    {
        PlayerEventType EventType
        {
            get;
        }
    }

    public struct PlayerInputUpdateEvent : IPlayerEvent
    {
        public PlayerEventType EventType
        {
            get => PlayerEventType.Input;
        }

        public Vector2 directional;
        public PlayerInputButton button;
        public ButtonPhase phase;

        public PlayerInputUpdateEvent(PlayerInputButton button, ButtonPhase phase, Rewired.Player player)
        {
            this.button = button;
            this.phase = phase;
            directional = new Vector2(player.GetAxisRaw(Input.Constants.Action.Horizontal), player.GetAxisRaw(Input.Constants.Action.Vertical));
        }

        public PlayerInputUpdateEvent(Rewired.Player player)
        {
            directional = new Vector2(player.GetAxis(Input.Constants.Action.Horizontal), player.GetAxis(Input.Constants.Action.Vertical));
            this.button = PlayerInputButton.None;
            this.phase = ButtonPhase.NotPressed;
        }
    }

    public struct PlayerJumpEvent : IPlayerEvent
    {
        public PlayerEventType EventType
        {
            get => PlayerEventType.Jump;
        }
    }

    public struct PlayerDirectionFacingUpdateEvent : IPlayerEvent
    {
        public PlayerDirectionFacingUpdateEvent(bool facingForwards)
        {
            FacingForwards = facingForwards;
        }

        public PlayerEventType EventType
        {
            get => PlayerEventType.DirectionFacingUpdate;
        }

        public bool FacingForwards
        {
            get; private set;
        }
    }

    public struct PlayerGroundStateUpdateEvent : IPlayerEvent
    {
        public PlayerGroundStateUpdateEvent(bool isGrounded)
        {
            IsGrounded = isGrounded;
        }

        public PlayerEventType EventType
        {
            get => PlayerEventType.GroundStateUpdate;
        }

        public bool IsGrounded
        {
            get;
            private set;
        }
    }

    public struct PlayerAttackEvent : IPlayerEvent
    {
        public PlayerAttackEvent(AttackType type, AttackDirection direction)
        {
            Type = type;
            Direction = direction;
        }

        public PlayerEventType EventType
        {
            get => PlayerEventType.Attack;
        }

        public AttackType Type
        {
            get; private set;
        }

        public AttackDirection Direction
        {
            get; private set;
        }
    }

    public struct PlayerAnimationStateEvent : IPlayerEvent
    {
        public PlayerAnimationStateEvent(PlayerAnimationStateEventType type)
        {
            Type = type;
        }

        public PlayerEventType EventType
        {
            get => PlayerEventType.AnimationState;
        }

        public PlayerAnimationStateEventType Type
        {
            get;
            private set;
        }
    }

    public struct PlayerAnimationEvent : IPlayerEvent
    {
        public PlayerEventType EventType
        {
            get => PlayerEventType.AnimationEvent;
        }
    }
}