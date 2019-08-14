namespace Duel.PlayerSystems
{
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

    public interface IAnimationStateEventSubscriber
    {
        void OnAnimationStateEvent(PlayerAnimationStateEvent eventArgs);
    }

    public interface IAnimationEventSubscriber
    {
        void OnAnimationEvent(PlayerAnimationEvent eventArgs);
    }

    public interface IHitSubscriber
    {
        void OnHit(PlayerHitEvent eventArgs);
    }

    public delegate void GroundStateUpdateSubscription(PlayerGroundStateUpdateEvent eventArgs);

    public delegate void JumpSubscription(PlayerJumpEvent eventArgs);

    public delegate void InputUpdateSubscription(PlayerInputUpdateEvent eventArgs);

    public delegate void AttackSubscription(PlayerAttackEvent eventArgs);

    public delegate void DirectionFacingUpdateSubscription(PlayerDirectionFacingUpdateEvent eventArgs);

    public delegate void AnimationStateEventSubscription(PlayerAnimationStateEvent eventArgs);

    public delegate void AnimationEventSubscription(PlayerAnimationEvent eventArgs);

    public delegate void HitSubscription(PlayerHitEvent eventArgs);
}