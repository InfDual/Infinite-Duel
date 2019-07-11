namespace Duel.PlayerSystems
{
    public enum PlayerEventType
    {
        Input,
        Jump,
        GroundStateUpdate
    }

    public interface IJumpUpdateSubscriber
    {
        void OnJump(PlayerJumpEvent eventArgs);
    }

    public interface IInputUpdateSubscriber
    {
        void OnInputUpdate(PlayerInputUpdateEvent eventArgs);
    }

    public interface IGroundStateSubscriber
    {
        void OnGroundStateUpdate(PlayerGroundStateUpdateEvent eventArgs);
    }

    public static class PlayerEventExtensionMethods
    {
        public static PlayerEventType GetSubscribedEventType(this IJumpUpdateSubscriber subscriber)
        {
            return PlayerEventType.Jump;
        }

        public static PlayerEventType GetSubscribedEventType(this IInputUpdateSubscriber subscriber)
        {
            return PlayerEventType.Input;
        }

        public static PlayerEventType GetSubscribedEventType(this IGroundStateSubscriber subscriber)
        {
            return PlayerEventType.GroundStateUpdate;
        }
    }

    public delegate void GroundStateUpdateSubscription(PlayerGroundStateUpdateEvent eventArgs);

    public delegate void JumpSubscription(PlayerJumpEvent eventArgs);

    public delegate void InputUpdateSubscription(PlayerInputUpdateEvent eventArgs);
}