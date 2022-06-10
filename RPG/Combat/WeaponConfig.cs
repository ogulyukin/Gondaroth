using System.Collections.Generic;
using RPG.Attributes;
using RPG.Inventories;
using RPG.Stats;
using UnityEngine;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "RPG/Inventory/Make Weapon item", order = 0)]
    public class WeaponConfig : EquipableItem, IModifierProvider
    {
        [SerializeField] private SkillNames skillForUse;
        [SerializeField] private Weapon weaponPrefab;
        [SerializeField] private Projectile projectile;
        [SerializeField] private AnimatorOverrideController animatorOverride;
        [SerializeField] private float weaponDamage;
        [SerializeField] private float weaponParryChance = 0;
        [SerializeField] private float avoidChance = 50f;
        [SerializeField] private float weaponRange;
        [SerializeField] private float weaponTimeout;
        [SerializeField] private int percentageModifier = 1;
        [SerializeField] private bool isRightHanded = true;

        private const string WeaponName = "Weapon";

        public SkillNames GetSkillForUse()
        {
            return skillForUse;
        }

        public bool CanAvoid()
        {
            return weaponPrefab == null ? true : false;
        }

        public float GetWeaponDamage()
        {
            return weaponDamage;
        }
        public Weapon Spawn(Transform rightHandTransform, Transform leftHandTransform, Animator animator)
        {
            DestroyOldWeapon(rightHandTransform, leftHandTransform);

            Weapon weapon = null;
            if (weaponPrefab != null)
            {
                weapon = Instantiate(weaponPrefab, isRightHanded ? rightHandTransform : leftHandTransform);
                weapon.gameObject.name = WeaponName;
            }
            var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
            if (animatorOverride != null)
            {
                animator.runtimeAnimatorController = animatorOverride;
            }
            else if (overrideController != null)
            {
                animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
            }

            return weapon;
        }
        private void DestroyOldWeapon(Transform rightHand, Transform leftHand)
        {
            var oldWeapon = rightHand.Find(WeaponName);
            if (oldWeapon == null) oldWeapon = leftHand.Find(WeaponName);
            if (oldWeapon != null)
            {
                oldWeapon.name = "Destroyed";
                Destroy(oldWeapon.gameObject);
            }
        }
        
        public float GetWeaponRange()
        {
            return weaponRange;
        }
        public float GetWeaponTimeout()
        {
            return weaponTimeout;
        }

        public bool HasProjectile()
        {
            return projectile != null;
        }

        public void LaunchProjectile(Health target, Transform rightHand, Transform leftHand, GameObject instigator, float damage)
        {
            var projectileInstance = Instantiate(projectile,isRightHanded ? rightHand.position : leftHand.position, Quaternion.identity);
            projectileInstance.SetRange(weaponRange);
            projectileInstance.SetTarget(target, instigator, damage);
        }

        public float GetWeaponPercentageModifier()
        {
            return percentageModifier;
        }

        public IEnumerable<float> GetAdditiveModifier(MainStats stat)
        {
            if(stat == MainStats.Damage) yield return weaponDamage;
            if(stat == MainStats.Parry) yield return weaponParryChance;
            if(stat == MainStats.Evade) yield return avoidChance;
        }

        public IEnumerable<float> GetPercentageModifier(MainStats stat)
        {
            if(stat == MainStats.Damage) yield return percentageModifier;
        }

        public bool GetWeaponHand()
        {
            return isRightHanded;
        }
    }
    
}


