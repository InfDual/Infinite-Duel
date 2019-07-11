using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Duel.PlayerSystems
{
    public class PlayerPhysics : PlayerModule, IInputUpdateSubscriber
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

        private bool falling;

        [SerializeField]
        private Rigidbody2D rb2d;

        private bool grounded;

        private ContactPoint2D[] currentContacts = new ContactPoint2D[5];

        protected override void OnInitialize()
        {
            rb2d = GetComponent<Rigidbody2D>();
        }

        private void ProcessInput(PlayerInputUpdateEvent playerEvent)
        {
        }

        private void AttemptJump()
        {
            if (grounded)
            {
                rb2d.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                master.InvokePlayerEvent(new PlayerJumpEvent());
            }
        }

        private void FixedUpdate()
        {
            if (falling)
            {
                rb2d.velocity = new Vector2(rb2d.velocity.x, Mathf.Max((rb2d.velocity.y - gravity * Time.fixedDeltaTime), maxFallSpeed));
            }
        }

        public void OnInputUpdate(PlayerInputUpdateEvent playerEvent)
        {
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
    }
}