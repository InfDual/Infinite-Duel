using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Duel.Combat;

namespace Duel.PlayerSystems
{
    public class PlayerCombat : PlayerModule, IAnimationEventSubscriber
    {
        public CharacterAnimationRegistryObject animationRegistryObject;

        [SerializeField]
        private Transform colliderContainer;

        [SerializeField]
        private Animator anim;

        [SerializeField]
        private List<BoxCollider2D> collisionBoxes = new List<BoxCollider2D>();

        public void OnAnimationEvent(PlayerAnimationEvent eventArgs)
        {
            if (eventArgs.Type == PlayerAnimationEventType.AttackKeyFrame)
            {
                int attackIndex = Utilities.GetByte(ref eventArgs.intBytes, 1);
                int frameIndex = Utilities.GetByte(ref eventArgs.intBytes, 2);
                UpdateCollisionBoxes(animationRegistryObject[attackIndex].frameData[frameIndex]);
            }
        }

        public void UpdateCollisionBoxes(FrameData frameData)
        {
            while (collisionBoxes.Count < frameData.BoxCount)
            {
                GameObject newColl = new GameObject("Collision Box", typeof(BoxCollider2D));
                newColl.transform.parent = colliderContainer;
                newColl.layer = Layers.hitbox;
                collisionBoxes.Add(newColl.GetComponent<BoxCollider2D>());
            }

            int lastIndex = 0;
            for (int i = 0; i < frameData.BoxCount; i++, lastIndex++)
            {
                collisionBoxes[i].enabled = true;
                collisionBoxes[i].gameObject.layer = frameData[i].Type == 0 ? Layers.hitbox : Layers.hurtbox;
                collisionBoxes[i].transform.localPosition = frameData[i].position;
                collisionBoxes[i].transform.localScale = frameData[i].size;
                collisionBoxes[i].transform.localEulerAngles = Vector3.forward * frameData[i].rotation;
            }
            for (int i = lastIndex; i < collisionBoxes.Count; i++)
            {
                collisionBoxes[i].enabled = false;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            print(collision.gameObject.name);
        }
    }
}