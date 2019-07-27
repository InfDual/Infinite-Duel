using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Duel.Combat;

namespace Duel.PlayerSystems
{
    public class PlayerCombat : PlayerModule
    {
        public CharacterAttackRegistryObject attackRegistryObject;

        [SerializeField]
        private Transform colliderContainer;

        [SerializeField]
        private Animator anim;

        [SerializeField]
        private List<BoxCollider2D> collisionBoxes = new List<BoxCollider2D>();

        public void UpdateCollisionBoxes(FrameData frameData)
        {
            int hurtboxCount = frameData.hurtBoxes.Count;
            int hitboxCount = frameData.hitBoxes.Count;
            if (collisionBoxes.Count < hurtboxCount + hitboxCount)
            {
                int difference = (hurtboxCount + hitboxCount) - collisionBoxes.Count;
                for (int i = 0; i < difference; i++)
                {
                    GameObject newColl = new GameObject("Collision Box", typeof(BoxCollider2D));
                    newColl.transform.parent = colliderContainer;
                    collisionBoxes.Add(newColl.GetComponent<BoxCollider2D>());
                }
            }

            int lastIndex = 0;
            for (int i = 0; i < hurtboxCount; i++)
            {
                lastIndex = i;
                collisionBoxes[i].offset = frameData.hurtBoxes[i].position;
                collisionBoxes[i].size = frameData.hurtBoxes[i].size;
                collisionBoxes[i].transform.localEulerAngles = Vector3.forward * frameData.hurtBoxes[i].rotation;
            }
            for (int i = 0; i < hitboxCount; i++)
            {
                collisionBoxes[lastIndex + i].transform.localPosition = frameData.hitBoxes[i].position;
                collisionBoxes[lastIndex + i].transform.localScale = ((Vector3)frameData.hitBoxes[i].size) + Vector3.forward;
                collisionBoxes[lastIndex + i].transform.localEulerAngles = Vector3.forward * frameData.hitBoxes[i].rotation;
            }
        }
    }
}