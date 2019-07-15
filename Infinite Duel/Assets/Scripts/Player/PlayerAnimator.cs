using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Duel.PlayerSystems
{
    public class PlayerAnimator : PlayerModule, IJumpUpdateSubscriber, IInputUpdateSubscriber, IGroundStateSubscriber, IAttackSubscriber, IAnimationStateEventSubscriber
    {
        [SerializeField]
        private SpriteRenderer spriteRenderer;

        [SerializeField]
        private Animator anim;

        private bool attacking;

        #region Parameter Hashes

        private int jumpHash;
        private int landHash;
        private int attackHash;
        private int blockingHash;
        private int airborneHash;
        private int movingHash;
        private int inputXHash;
        private int inputYHash;
        private int attackDirectionHash;

        #endregion Parameter Hashes

        protected override void OnInitialize()
        {
            jumpHash = Animator.StringToHash("Jump");
            landHash = Animator.StringToHash("Land");
            attackHash = Animator.StringToHash("Attack");
            blockingHash = Animator.StringToHash("Blocking");
            airborneHash = Animator.StringToHash("Airborne");
            movingHash = Animator.StringToHash("Moving");
            inputXHash = Animator.StringToHash("InputX");
            inputYHash = Animator.StringToHash("InputY");
            attackDirectionHash = Animator.StringToHash("AttackDirection");
        }

        public void OnJump(PlayerJumpEvent eventArgs)
        {
            anim.SetTrigger(jumpHash);
        }

        public void OnInputUpdate(PlayerInputUpdateEvent playerInput)
        {
            if (attacking)
                return;
            SetDirectionFacing(playerInput);

            anim.SetBool(movingHash, playerInput.directional.x != 0);
        }

        private void SetDirectionFacing(PlayerInputUpdateEvent playerInput)
        {
            bool oldFlip = spriteRenderer.flipX;
            if (playerInput.directional.x > 0)
            {
                spriteRenderer.flipX = false;
            }
            else if (playerInput.directional.x < 0)
            {
                spriteRenderer.flipX = true;
            }

            if (spriteRenderer.flipX != oldFlip)
            {
                master.InvokePlayerEvent(new PlayerDirectionFacingUpdateEvent(!spriteRenderer.flipX));
            }
        }

        public void OnGroundStateUpdate(PlayerGroundStateUpdateEvent eventArgs)
        {
            anim.SetBool(airborneHash, !eventArgs.IsGrounded);
        }

        public void OnAttack(PlayerAttackEvent eventArgs)
        {
            if (attacking)
                return;
            anim.SetInteger(attackDirectionHash, (int)eventArgs.Direction);
            anim.SetTrigger(attackHash);
        }

        public void OnAnimationStateEvent(PlayerAnimationStateEvent eventArgs)
        {
            if (eventArgs.Type == PlayerAnimationStateEventType.AttackStart)
                attacking = true;
            else if (eventArgs.Type == PlayerAnimationStateEventType.AttackEnd)
                attacking = false;
        }
    }
}