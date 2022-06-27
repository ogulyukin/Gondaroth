using RPG.Attributes;
using RPG.Core;
using UnityEngine;

namespace RPG.Magic
{
    public class MagicControl : MonoBehaviour, IAction
    {
        [SerializeField] private GameObject currentSpellPrefab;
        private ISpell _currentSpell;
        private Animator _animator;
        private Mana _mana;
        private Health _health;
        private ActionScheduler _actionScheduler;
        private static readonly int Cast01 = Animator.StringToHash("Cast01");
        private static readonly int StopCasting = Animator.StringToHash("StopCasting");
        private static readonly int Cast2Start = Animator.StringToHash("Cast2Start");


        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _health = GetComponent<Health>();
            _mana = GetComponent<Mana>();
            _actionScheduler = GetComponent<ActionScheduler>();
            //temp method!!!!
            ChangeCurrentSpell(currentSpellPrefab.GetComponent<ISpell>());
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                if(!CanCastSpell()) return;
                TriggerCast();
            } 
        }
        public bool CanCastSpell()
        {
            return (_health.IsAlive() && _currentSpell.CanCast(_mana.GetCurrentManaLevel()));
        }
        public void Cancel()
        {
            _animator.ResetTrigger(Cast01);
            _animator.ResetTrigger(Cast2Start);
            _animator.SetTrigger(StopCasting);
        }
    
        public void TriggerCast()
        {
            if(ReferenceEquals(_currentSpell, null)) return;
            _actionScheduler.StartAction(this);
            _animator.ResetTrigger(StopCasting);
            _animator.SetTrigger(_currentSpell.GetAnimationWay() ? Cast01 : Cast2Start);
            _mana.SpendMana(_currentSpell.GetManaCost());
        }

        private void ChangeCurrentSpell(ISpell spell)
        {
            _currentSpell = spell;
        }
        //Animation Event
        public void SuccessfulSpellCasting()
        {
            _currentSpell.Cast(this.transform, this.transform);
        }

    }
}
