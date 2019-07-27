using System.Collections;
using System.Collections.Generic;
using Duel.PlayerSystems;
using UnityEngine;

namespace Duel.Combat
{
    [System.Serializable]
    public class AttackInfo
    {
        public AnimationClip attackAnimation;
        public AttackDirection direction;
        public List<FrameData> keyFrames = new List<FrameData>();
    }

    [System.Serializable]
    public class FrameData
    {
        public List<HitboxInfo> hitBoxes = new List<HitboxInfo>();
        public List<HurtboxInfo> hurtBoxes = new List<HurtboxInfo>();
    }

    [System.Serializable]
    public class HitboxInfo : BoxInfo
    {
        public float damage;
        public Vector2 knockbackDirection;
        public float knockbackForce;
    }

    [System.Serializable]
    public class BoxInfo
    {
        public Vector2 position;
        public Vector2 size = Vector2.one;
        public float rotation;
    }

    [System.Serializable]
    public class HurtboxInfo : BoxInfo
    {
        public bool superArmor;
    }

    public static class CombatExtensions
    {
        public static string[] GetAttackAnimationNames(this CharacterAttackRegistryObject playerCombat)
        {
            List<string> names = new List<string>();

            for (int i = 0; i < playerCombat.AttackCount; i++)
            {
                names.Add(playerCombat[i].attackAnimation.name);
            }
            return names.ToArray();
        }
    }
}