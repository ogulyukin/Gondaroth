using UnityEngine;

namespace RPG.Magic
{
    public interface ISpell
    {
        public void Cast(Transform target, Transform caster);
        public bool CanCast(float mana);

        public bool GetAnimationWay();

        public float GetManaCost();
    }
}
