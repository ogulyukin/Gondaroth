using RPG.Attributes;
using UnityEngine;

namespace RPG.Magic
{
    public class SpellHealing : SpellTemplate, ISpell
    {
        public void Cast(Transform target, Transform caster)
        {
            caster.GetComponent<Health>().Heal(healEffect);
        }

        public bool CanCast(float mana)
        {
            return mana >= manaCost;
        }
    }
}
