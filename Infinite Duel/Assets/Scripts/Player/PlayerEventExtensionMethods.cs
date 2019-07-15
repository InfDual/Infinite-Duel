namespace Duel.PlayerSystems
{
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
}