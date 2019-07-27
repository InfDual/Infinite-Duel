using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Duel.PlayerSystems
{
    public class PlayerPhysics : PlayerModule, IInputUpdateSubscriber, IAnimationStateEventSubscriber
    {
        [InfoBox("Currently Useless")]
        [SerializeField]
        private float gravity;

        [InfoBox("Currently Useless")]
        [SerializeField]
        private float maxFallSpeed;

        [SerializeField]
        private float moveSpeed;

        [SerializeField]
        private float jumpForce;

        [SerializeField]
        private Rigidbody2D rb2d;

        private bool grounded;

        private ContactPoint2D[] currentContacts = new ContactPoint2D[5];
        private bool attacking = false;

        protected override void OnInitialize()
        {
            rb2d = GetComponent<Rigidbody2D>();
        }

        private void AttemptJump()
        {
            if (grounded && !attacking)
            {
                rb2d.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                master.InvokePlayerEvent(new PlayerJumpEvent());
            }
        }

        public void OnInputUpdate(PlayerInputUpdateEvent playerEvent)
        {
            if (rb2d == null)
                return;
            if (!attacking)
                rb2d.velocity = new Vector2(moveSpeed * playerEvent.directional.x, rb2d.velocity.y);
            if (playerEvent.button == PlayerInputButton.Jump)
            {
                AttemptJump();
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            UpdateGroundState();
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            UpdateGroundState();
        }

        private void UpdateGroundState()
        {
            int contactCount = rb2d.GetContacts(currentContacts);
            bool oldGroundedValue = grounded;
            grounded = false;
            for (int i = 0; i < contactCount; i++)
            {
                grounded = currentContacts[i].collider.CompareTag("Terrain");
                if (grounded)
                    break;
            }
            if (grounded != oldGroundedValue)
            {
                master.InvokePlayerEvent(new PlayerGroundStateUpdateEvent(grounded));
            }
        }

        public void OnAnimationStateEvent(PlayerAnimationStateEvent eventArgs)
        {
            if (eventArgs.Type == PlayerAnimationStateEventType.AttackStart)
            {
                attacking = true;
                if (grounded)
                    rb2d.velocity *= Vector2.up;
            }
            else if (eventArgs.Type == PlayerAnimationStateEventType.AttackEnd)
                attacking = false;
        }
    }
}