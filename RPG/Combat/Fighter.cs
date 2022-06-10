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
        private SkillNames _defenceSkill;
        private static readonly int CombatStyle = Animator.StringToHash("CombatStyle");


        private void Awake()
        {
            _equipment = GetComponent<Equipment>();
            if (_equipment)
            {
                _equipment.equipmentUpdated += UpdateWeapon;
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
            if (_isDefending)
            {
                //transform.LookAt(_target.transform);
                StartDefenceAnimation();
                _isDefending = false;
                return;
            }
            if(_target == null || !_target.IsAlive()) return;
            if (!GetIsInRange(_target.transform))
            {
                GetComponent<Mover>().Moveto(_target.transform.position);
            }
            else
            {
                GetComponent<Mover>().Cancel();
                AttackBehaviour();
            }
        }

        public void EquipWeapon(WeaponConfig weaponConfig)
        {
            _currentWeaponConfig = weaponConfig;
            _currentWeapon = weaponConfig.Spawn(rightHandTransform, leftHandTransform,GetComponent<Animator>());
        }
        
        private void StartDefenceAnimation()
        {
            GetComponent<Animator>().ResetTrigger(StopAttack1);
            switch (_defenceSkill)
            {
                case SkillNames.Block:
                    GetComponent<Animator>().SetTrigger(Block1Start);
                    break;
                case SkillNames.Parring:
                    GetComponent<Animator>().SetTrigger(Parry);
                    break;
                case SkillNames.Evade:
                    GetComponent<Animator>().SetTrigger(Dodge);
                    break;
            }
            Debug.Log($"{gameObject.name} Start defence animation {_defenceSkill}");
            _animationInProgress = true;
            _timeSinceAnimationStart = 0;
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
            GetComponent<Animator>().ResetTrigger(StopAttack1);
            GetComponent<Animator>().SetTrigger(Attack1);
            _animationInProgress = true;
            _timeSinceAnimationStart = 0;
            Debug.Log($"{gameObject.name} Start attack animation");
        }

        private bool GetIsInRange(Transform targetTransform)
        {
            return Vector3.Distance(transform.position, targetTransform.position) < _currentWeaponConfig.GetWeaponRange();
        }

        public void Attack(CombatTarget combatTarget)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            _target = combatTarget.GetComponent<Health>();
            GetComponent<Animator>().SetFloat(CombatStyle, 1.0f);
        }

        public void Cancel()
        {
            _target = null;
            _animationInProgress = false;
            StopAttack();
            GetComponent<Animator>().SetFloat(CombatStyle, 1.0f);
        }

        public string GetEnemyHealth()
        {
            return _target != null ? _target.GetHealthDetail() : "N/A";
        }
        public bool CanAttack(CombatTarget combat)
        {
            if (combat == null || (!GetComponent<Mover>().CanMoveTo(combat.transform.position) && !GetIsInRange(combat.transform))) return false;
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
        private void StopAttack()
        {
            GetComponent<Animator>().ResetTrigger(Attack1);
            GetComponent<Animator>().ResetTrigger(Block1Start);
            GetComponent<Animator>().ResetTrigger(Parry);
            GetComponent<Animator>().ResetTrigger(Dodge);
            GetComponent<Animator>().SetTrigger(StopAttack1);
            GetComponent<Mover>().Cancel();
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
                Debug.Log($"{gameObject.name} Damage: {damage} Chance to Hit: {chanceToHit}");
                if(_target != null) _target.GetComponent<Defence>().GetAttack(chanceToHit, damage, gameObject);//_target.TakeDamage(gameObject, damage);
            }
        }
        //Animation events
        private void Shoot()
        {
            Hit();
        }

        private void AnimationEnded()
        {
            Debug.Log($"{gameObject.name} Animation ended");
            _animationInProgress = false;
        }

        public void Defend(SkillNames skill)
        {
            _isDefending = true;
            Debug.Log($"{gameObject.name}: {skill}");
            _defenceSkill = skill;
        }
    }
}
