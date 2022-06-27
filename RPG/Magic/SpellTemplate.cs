using UnityEngine;

namespace RPG.Magic
{
    public class SpellTemplate : MonoBehaviour
    {
        [SerializeField] protected float manaCost;
        [SerializeField] protected bool mainAnimation;
        
        [SerializeField] protected SpellEffectTargets targets;
        [SerializeField] protected float duration;
        [SerializeField] protected bool stunningEffect;
        [SerializeField] protected bool charmEffect;
        [SerializeField] protected bool silenceEffect;
        [SerializeField] protected bool sleepEffect;
        [SerializeField] protected float healEffect;
        [SerializeField] protected float damageEffect;
        
        public bool GetAnimationWay()
        {
            return mainAnimation;
        }

        public float GetManaCost()
        {
            return manaCost;
        }
    }
}
