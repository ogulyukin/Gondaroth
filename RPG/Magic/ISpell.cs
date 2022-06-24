using UnityEngine;

namespace RPG.Magic
{
    public interface ISpell
    {
        public float Cast(Transform target, Transform caster);
        public bool CanCast(float mana);

        public bool GetAnimationWay();
    }
}
