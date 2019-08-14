using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Duel.Combat
{
    [CreateAssetMenu]
    public class CharacterAnimationRegistryObject : SerializedScriptableObject
    {
        [Sirenix.Serialization.OdinSerialize, NonSerialized]
        public List<AnimationInfo> animations = new List<AnimationInfo>();

        public int AnimationCount
        {
            get
            {
                return animations.Count;
            }
        }

        public AnimationInfo this[int index]
        {
            get
            {
                return animations[index];
            }
        }

        [Button]
        public void InitializeFromAnimator(RuntimeAnimatorController anim)
        {
            foreach (var item in anim.animationClips)
            {
                animations.Add(new AnimationInfo() { clip = item });
            }
        }
    }
}