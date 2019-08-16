using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Duel.Combat;

namespace Duel.PlayerSystems
{
    public class PlayerCombat : PlayerModule, IAnimationEventSubscriber, IHitSubscriber
    {
        public CharacterAnimationRegistryObject animationRegistryObject;

        [SerializeField]
        private Transform colliderContainer;

        [SerializeField]
        private Animator anim;

        [SerializeField]
        private List<CombatBox> collisionBoxes = new List<CombatBox>();

        public void OnAnimationEvent(PlayerAnimationEvent eventArgs)
        {
            if (eventArgs.Type == PlayerAnimationEventType.AttackKeyFrame)
            {
                int attackIndex = Utilities.GetByte(ref eventArgs.intBytes, 1);
                int frameIndex = Utilities.GetByte(ref eventArgs.intBytes, 2);

                UpdateCollisionBoxes(animationRegistryObject[attackIndex].frameData[frameIndex]);
            }
        }

        public void OnHit(PlayerHitEvent eventArgs)
        {
            Debug.Log($"Damage Dealt : {eventArgs.hitboxInfo.damage}");
            Debug.Log($"Damage Taken : {eventArgs.hitboxInfo.damage * (1 - eventArgs.hurtboxInfo.damageMitigation)}");
        }

        public void UpdateCollisionBoxes(FrameData frameData)
        {
            while (collisionBoxes.Count < frameData.BoxCount)
            {
                collisionBoxes.Add(CombatBox.CreateCombatBox(colliderContainer));
            }

            int lastIndex = 0;
            for (int i = 0; i < frameData.BoxCount; i++, lastIndex++)
            {
                collisionBoxes[i].Activate(frameData[i]);
            }
            for (int i = lastIndex; i < collisionBoxes.Count; i++)
            {
                collisionBoxes[i].Deactivate();
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider.CompareTag("Hitbox"))
            {
                master.InvokePlayerEvent(new PlayerHitEvent((HitboxInfo)collision.collider.GetComponent<CombatBox>().activeBox, (HurtboxInfo)collision.otherCollider.GetComponent<CombatBox>().activeBox));
            }
        }
    }
}