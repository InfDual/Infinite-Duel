namespace Duel.PlayerSystems
{
    public enum PlayerEventType
    {
        Input,
        Jump,
        Attack,
        GroundStateUpdate,
        DirectionFacingUpdate
    }

    public interface IJumpUpdateSubscriber
    {
        void OnJump(PlayerJumpEvent eventArgs);
    }

    public interface IDirectionFacingUpdateSubscriber
    {
        void OnDirectionFacingUpdate(PlayerDirectionFacingUpdateEvent eventArgs);
    }

    public interface IInputUpdateSubscriber
    {
        void OnInputUpdate(PlayerInputUpdateEvent eventArgs);
    }

    public interface IGroundStateSubscriber
    {
        void OnGroundStateUpdate(PlayerGroundStateUpdateEvent eventArgs);
    }

    public interface IAttackSubscriber
    {
        void OnAttack(PlayerAttackEvent eventArgs);
    }

    public static class PlayerEventExtensionMethods
    {
        public static PlayerEventType GetSubscribedEventType(this IJumpUpdateSubscriber subscriber)
        {
            return PlayerEventType.Jump;
        }

        public static PlayerEventType GetSubscribedEventType(this IDirectionFacingUpdateSubscriber subscriber)
        {
            return PlayerEventType.DirectionFacingUpdate;
        }

        public static PlayerEventType GetSubscribedEventType(this IInputUpdateSubscriber subscriber)
        {
            return PlayerEventType.Input;
        }

        public static PlayerEventType GetSubscribedEventType(this IGroundStateSubscriber subscriber)
        {
            return PlayerEventType.GroundStateUpdate;
        }

        public static PlayerEventType GetSubscribedEventType(this IAttackSubscriber subscriber)
        {
            return PlayerEventType.Attack;
        }
    }

    public delegate void GroundStateUpdateSubscription(PlayerGroundStateUpdateEvent eventArgs);

    public delegate void JumpSubscription(PlayerJumpEvent eventArgs);

    public delegate void InputUpdateSubscription(PlayerInputUpdateEvent eventArgs);

    public delegate void AttackSubscription(PlayerAttackEvent eventArgs);

    public delegate void DirectionFacingUpdateSubscription(PlayerDirectionFacingUpdateEvent eventArgs);
}