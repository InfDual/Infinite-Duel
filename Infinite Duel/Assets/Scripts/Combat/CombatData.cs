using System.Collections;
using System.Collections.Generic;
using Duel.PlayerSystems;
using UnityEngine;

namespace Duel.Combat
{
    public enum BoxType
    {
        Hit,
        Hurt,
        Super
    }

    [System.Serializable]
    public class AnimationInfo
    {
        public AnimationClip clip;
        public List<FrameData> frameData = new List<FrameData>();
    }

    public class FrameData
    {
        public BoxInfo this[int index]
        {
            get
            {
                return index < boxes.Count && index >= 0 ? boxes[index] : null;
            }
        }

        public int BoxCount
        {
            get
            {
                return boxes != null ? boxes.Count : 0;
            }
        }

        public int HitboxCount
        {
            get => hitboxCount;
            private set => hitboxCount = value;
        }

        public int HurtboxCount
        {
            get => hurtboxCount;
            private set => hurtboxCount = value;
        }

        public List<BoxInfo> boxes = new List<BoxInfo>();

        [SerializeField, HideInInspector]
        private int hitboxCount;

        [SerializeField, HideInInspector]
        private int hurtboxCount;

        public void AddBox(BoxInfo box)
        {
            if (box is HitboxInfo)
                HitboxCount++;
            else
                HurtboxCount++;
            boxes.Add(box);
        }

        public void Remove(BoxInfo box)
        {
            if (boxes.Contains(box))
            {
                if (box.Type == BoxType.Hit)
                    HitboxCount--;
                else
                    HurtboxCount--;

                boxes.Remove(box);
            }
        }
    }

    public abstract class BoxInfo
    {
        public abstract BoxType Type
        {
            get;
        }

        public Vector2 position;
        public Vector2 size = Vector2.one;
        public float rotation;
    }

    public class HitboxInfo : BoxInfo
    {
        public override BoxType Type => BoxType.Hit;
        public float damage;
        public float hitStun;
        public Vector2 knockbackDirection;
        public float knockbackForce;
    }

    public class HurtboxInfo : BoxInfo
    {
        public override BoxType Type => superArmor ? BoxType.Super : BoxType.Hurt;

        public float hitstunMitigation;
        public float damageMitigation;
        public bool superArmor;
    }

    public static class CombatExtensions
    {
        public static string[] GetAnimationClipNames(this CharacterAnimationRegistryObject animationRegistry)
        {
            List<string> names = new List<string>();

            for (int i = 0; i < animationRegistry.animations.Count; i++)
            {
                names.Add(animationRegistry.animations[i].clip.name);
            }
            return names.ToArray();
        }
    }
}