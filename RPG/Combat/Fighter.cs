using RPG.Attributes;
using RPG.Core;
using RPG.Inventories;
using RPG.Movement;
using RPG.Stats;
using Saving;
using UnityEngine;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, ISaveable
    {
        [SerializeField] private Transform rightHandTransform;
        [SerializeField] private Transform leftHandTransform;
        [SerializeField] private WeaponConfig defaultWeaponConfig;
        [SerializeField] private float maxAnimationTime = 1.5f;
        private Health _target;
        private float _timeSinceLastAttack;
        private float _timeSinceAnimationStart;
        private static readonly int Attack1 = Animator.StringToHash("Attack01");
        private static readonly int StopAttack1 = Animator.StringToHash("StopAttack");
        private static readonly int Block1Start = Animator.StringToHash("Block1Start");
        private static readonly int Parry = Animator.StringToHash("Parry");
        private static readonly int Dodge = Animator.StringToHash("Dodge");
        private WeaponConfig _currentWeaponConfig;
        private Weapon _currentWeapon;
        private Equipment _equipment;
        private BaseStats _baseStats;
        private bool _animationInProgress;
        private bool _isDefending;
        private bool _gotHit;
        private Animator _animator;
        private Mover _mover;
        private SkillNames _defenceSkill;
        private string _currentAttackId;
        private static readonly int CombatStyle = Animator.StringToHash("CombatStyle");
        private static readonly int StopDefence = Animator.StringToHash("StopDefence");
        private static readonly int GotHit1 = Animator.StringToHash("GotHit");
        private static readonly int StopGotHit = Animator.StringToHash("StopGotHit");


        private void Awake()
        {
            _equipment = GetComponent<Equipment>();
            _animator = GetComponent<Animator>();
            _mover = GetComponent<Mover>();
            if (_equipment)
            {
                _equipment.EquipmentUpdated += UpdateWeapon;
            }
        }
        
        private void Start()
        {
            EquipWeapon(_currentWeaponConfig == null ? defaultWeaponConfig : _currentWeaponConfig);
            _baseStats = GetComponent<BaseStats>();
        }

        private void Update()
        {
            _timeSinceLastAttack += Time.deltaTime;
            _timeSinceAnimationStart += Time.deltaTime;
            if (_timeSinceAnimationStart > maxAnimationTime) _animationInProgress = false;
            if(_animationInProgress) return;
            if (_gotHit)
            {
                StartGotHitAnimation();
                _gotHit = false;
                return;
            }
            if (_isDefending)
            {
                //transform.LookAt(_target.transform);
                StartDefenceAnimation();
                _isDefending = false;
                return;
            }
            if(ReferenceEquals(_target, null) || !_target.IsAlive()) return;
            if (!GetIsInRange(_target.transform))
            {
                _mover.Moveto(_target.transform.position);
            }
            else
            {
                _mover.Cancel();
                AttackBehaviour();
            }
        }

        public void EquipWeapon(WeaponConfig weaponConfig)
        {
            _currentWeaponConfig = weaponConfig;
            _currentWeapon = weaponConfig.Spawn(rightHandTransform, leftHandTransform,_animator);
        }

        public void GotHit()
        {
            _gotHit = true;
            //Debug.Log($"{gameObject.name}: got hit!!!");
            StartGotHitAnimation();
        }
        
        public void Attack(CombatTarget combatTarget)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            _target = combatTarget.GetComponent<Health>();
            _animator.SetFloat(CombatStyle, 1.0f);
        }
        
        public void Defend(SkillNames skill)
        {
            _isDefending = true;
            //Debug.Log($"{gameObject.name}: {skill}");
            _defenceSkill = skill;
        }

        public void Cancel()
        {
            _target = null;
            _animationInProgress = false;
            StopAttack();
            _animator.SetFloat(CombatStyle, 1.0f);
        }

        public string GetEnemyHealth()
        {
            return ReferenceEquals(_target, null) ? "N/A" : _target.GetHealthDetail();
        }
        public bool CanAttack(CombatTarget combat)
        {
            if (ReferenceEquals(combat, null) || (!_mover.CanMoveTo(combat.transform.position) && !GetIsInRange(combat.transform))) return false;
            return combat.GetComponent<Health>().IsAlive();
        }

        public object CaptureState()
        {
            return _currentWeaponConfig.name;
        }

        public void RestoreState(object state)
        {
            var weapon = Resources.Load<WeaponConfig>((string)state);
            EquipWeapon(weapon);
        }

        public bool CanAvoid()
        {
            return _currentWeaponConfig.CanAvoid();
        }

        public SkillNames GetWeaponSkill()
        {
            return _currentWeaponConfig.GetSkillForUse();
        }

        private void StartGotHitAnimation()
        {
            StopAttackAnimation();
            StopDefenceAnimation();
            _animator.ResetTrigger(StopGotHit);
            _animator.SetTrigger(GotHit1);
            //Debug.Log($"{gameObject.name} Start got hit animation");
            _animationInProgress = true;
            _timeSinceAnimationStart = 0;
            _timeSinceLastAttack += 5f;
        }
        private void StartDefenceAnimation()
        {
            _animator.ResetTrigger(StopDefence);
            StopAttackAnimation();
            switch (_defenceSkill)
            {
                case SkillNames.Block:
                    _animator.SetTrigger(Block1Start);
                    break;
                case SkillNames.Parring:
                    _animator.SetTrigger(Parry);
                    break;
                case SkillNames.Evade:
                    _animator.SetTrigger(Dodge);
                    break;
            }
            //Debug.Log($"{gameObject.name} Start defence animation {_defenceSkill}");
            _animationInProgress = true;
            _timeSinceAnimationStart = 0;
            _timeSinceLastAttack += 5f;
        }
        
        private void UpdateWeapon()
        {
            var weapon = _equipment.GetItemInSlot(EquipLocation.Weapon) as WeaponConfig;
            EquipWeapon(weapon == null ? defaultWeaponConfig : weapon);
        }

        private void AttackBehaviour()
        {
            transform.LookAt(_target.transform);
            if (_timeSinceLastAttack > _currentWeaponConfig.GetWeaponTimeout())
            {
                TriggerAttack();
                _timeSinceLastAttack = 0;
            }
        }

        private void TriggerAttack()
        {
            _animator.ResetTrigger(StopAttack1);
            _animator.SetTrigger(Attack1);
            _animationInProgress = true;
            _timeSinceAnimationStart = 0;
            //Debug.Log($"{gameObject.name} Start attack animation");
            //Tell target Defence try 
            if (!_currentWeaponConfig.HasProjectile())
            {
                var damage = GetComponent<BaseStats>().GetStat(MainStats.Damage) + _baseStats.GetStat(MainStats.Strength) / 10;
                var chanceToHit = _baseStats.GetStat(MainStats.Dexterity) / 10 +
                                  _baseStats.GetSkillLevel(_currentWeaponConfig.GetSkillForUse()) / 2;
                _currentAttackId = System.Guid.NewGuid().ToString();
                if(_target != null) _target.GetComponent<Defence>().GetAttack(chanceToHit, damage, gameObject, _currentAttackId);
            }
        }

        private bool GetIsInRange(Transform targetTransform)
        {
            return Vector3.Distance(transform.position, targetTransform.position) < _currentWeaponConfig.GetWeaponRange();
        }
        
        private void StopAttack()
        {
            StopDefenceAnimation();
            StopAttackAnimation();
            _animator.ResetTrigger(GotHit1);
            _animator.SetTrigger(StopGotHit);
            _mover.Cancel();
        }

        private void StopAttackAnimation()
        {
            _animator.ResetTrigger(Attack1);
            _animator.SetTrigger(StopAttack1);
        }

        private void StopDefenceAnimation()
        {
            _animator.ResetTrigger(Block1Start);
            _animator.ResetTrigger(Parry);
            _animator.ResetTrigger(Dodge);
            _animator.SetTrigger(StopDefence);
        }

        //Animation events
        private void Hit()
        {
            if(_target == null) return;
            if(_currentWeapon != null) _currentWeapon.OnHit();
            var damage = GetComponent<BaseStats>().GetStat(MainStats.Damage) + _baseStats.GetStat(MainStats.Strength) / 10;
            if (_baseStats.IsNpc()) damage += _currentWeaponConfig.GetWeaponDamage();
            if (_currentWeaponConfig.HasProjectile())
            {
                _currentWeaponConfig.LaunchProjectile(_target , rightHandTransform, leftHandTransform, gameObject, damage);
            }
            else
            {
                var chanceToHit = _baseStats.GetStat(MainStats.Dexterity) / 10 +
                                  _baseStats.GetSkillLevel(_currentWeaponConfig.GetSkillForUse()) / 2;
                //Debug.Log($"{gameObject.name} Damage: {damage} Chance to Hit: {chanceToHit}");
                if(_target != null) _target.GetComponent<Defence>().GotHit(_currentAttackId, damage, gameObject);//_target.TakeDamage(gameObject, damage);
            }
        }
        //Animation events
        private void Shoot()
        {
            Hit();
        }

        private void AnimationEnded()
        {
            //Debug.Log($"{gameObject.name} Animation ended");
            _animationInProgress = false;
        }
    }
}
