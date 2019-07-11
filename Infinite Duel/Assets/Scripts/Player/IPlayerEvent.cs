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

    public enum PlayerInputButton
    {
        Normal,
        Special,
        Throw,
        Block,
        Jump,
        Pause,
        None
    }

    public enum ButtonPhase
    {
        Down,
        Pressed,
        Up,
        NotPressed
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
}