using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Duel.Combat
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class CombatBox : MonoBehaviour
    {
        private Transform trans;
        private BoxCollider2D coll;

        public BoxInfo activeBox;

        private void Awake()
        {
            trans = transform;
            coll = GetComponent<BoxCollider2D>();
        }

        public void Activate(BoxInfo box)
        {
            activeBox = box;
            coll.enabled = true;
            gameObject.layer = box.Type == 0 ? Layers.hitbox : Layers.hurtbox;
            gameObject.name = box.Type == 0 ? "Hitbox" : "Hurtbox";
            gameObject.tag = box.Type == 0 ? "Hitbox" : "Hurtbox";
            trans.localPosition = box.position;
            trans.localScale = box.size;
            trans.localEulerAngles = new Vector3(0, 0, box.rotation);
        }

        public void Deactivate()
        {
            gameObject.name = "Disabled";
            coll.enabled = false;
        }

        public static CombatBox CreateCombatBox(Transform colliderContainer)
        {
            GameObject newColl = new GameObject("Collision Box", typeof(BoxCollider2D), typeof(CombatBox));
            newColl.transform.parent = colliderContainer;
            newColl.layer = Layers.hitbox;
            return newColl.GetComponent<CombatBox>();
        }
    }
}