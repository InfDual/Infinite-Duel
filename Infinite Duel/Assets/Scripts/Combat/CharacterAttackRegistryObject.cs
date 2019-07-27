using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Duel.Combat
{
    [CreateAssetMenu]
    public class CharacterAttackRegistryObject : ScriptableObject
    {
        public List<AttackInfo> groundAttacks = new List<AttackInfo>();
        public List<AttackInfo> airAttacks = new List<AttackInfo>();
        public List<AttackInfo> specialAttacks = new List<AttackInfo>();

        public int AttackCount
        {
            get
            {
                return groundAttacks.Count + airAttacks.Count + specialAttacks.Count;
            }
        }

        public AttackInfo this[int index]
        {
            get
            {
                AttackInfo attack = null;
                if (index < 0)
                    return attack;
                if (index < groundAttacks.Count)
                {
                    attack = groundAttacks[index];
                }
                else if (index < (groundAttacks.Count + airAttacks.Count))
                {
                    attack = airAttacks[index];
                }
                else if (index < (groundAttacks.Count + airAttacks.Count + specialAttacks.Count))
                {
                    attack = specialAttacks[index];
                }
                return attack;
            }
        }
    }
}