using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Duel.PlayerSystems
{
    public class PlayerAnimator : PlayerModule, IJumpUpdateSubscriber, IInputUpdateSubscriber, IGroundStateSubscriber
    {
        [SerializeField]
        private SpriteRenderer spriteRenderer;

        [SerializeField]
        private Animator anim;

        #region Parameter Hashes

        private int jumpHash;
        private int landHash;
        private int attackHash;
        private int blockingHash;
        private int airborneHash;
        private int movingHash;
        private int inputXHash;
        private int inputYHash;

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
        }

        public void OnJump(PlayerJumpEvent eventArgs)
        {
            anim.SetTrigger(jumpHash);
        }

        public void OnInputUpdate(PlayerInputUpdateEvent playerInput)
        {
            if (playerInput.directional.x > 0)
            {
                spriteRenderer.flipX = false;
            }
            else if (playerInput.directional.x < 0)
            {
                spriteRenderer.flipX = true;
            }

            anim.SetBool(movingHash, playerInput.directional.x != 0);
        }

        public void OnGroundStateUpdate(PlayerGroundStateUpdateEvent eventArgs)
        {
            anim.SetBool(airborneHash, !eventArgs.IsGrounded);
        }
    }
}