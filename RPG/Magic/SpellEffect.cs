using RPG.Core;
using UnityEngine;

namespace RPG.Magic
{
    public enum SpellEffectTargets
    {
        Friends,
        Enemy,
        All
    }
    public class SpellEffect : MonoBehaviour
    {
        public SpellEffectTargets targets;
        public float duration;
        public bool stunningEffect;
        public bool charmEffect;
        public bool silenceEffect;
        public bool sleepEffect;
        public float healEffect;
        public float damageEffect;
        public UnitTypes caster;
    }
}
