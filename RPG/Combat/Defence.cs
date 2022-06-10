using System;
using System.Collections.Generic;
using RPG.Attributes;
using RPG.Core;
using RPG.Inventories;
using RPG.Stats;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace RPG.Combat
{
    public class Defence : MonoBehaviour, IModifierProvider
    {
        [SerializeField] private float armor = 0; //for NPC
        [SerializeField] private float blockModifier = 0; //for NPC
        [SerializeField] private TMP_Text armorUIText; //for player
        private Fighter _fighter;
        private BaseStats _stats;
        private void Start()
        {
            _fighter = GetComponent<Fighter>();
            _stats = GetComponent<BaseStats>();
            if (armorUIText != null)
            {
                armorUIText.text = $"{_stats.GetStat(MainStats.Armor)}";
                GetComponent<Equipment>().equipmentUpdated += UpdateArmorUIText;
            }
        }

        public void GetAttack(float chance, float damage, GameObject instigator)
        {
            if (_fighter.CanAvoid())
            {
                TryDefenceSkill(chance, damage, instigator, SkillNames.Evade, MainStats.Evade);
                return;
            }

            var equipment = GetComponent<Equipment>();
            if (equipment != null)
            {
                if (equipment.IsItemEquipped(EquipLocation.Shield))
                {
                    TryDefenceSkill(chance, damage, instigator, SkillNames.Block, MainStats.Block);
                    return;
                }
            }

            if (blockModifier > 0)
            {
                TryDefenceSkill(chance, damage, instigator, SkillNames.Block, MainStats.Block);
                return;
            }
            TryDefenceSkill(chance, damage, instigator, SkillNames.Parring, MainStats.Parry);
        }

        private void UpdateArmorUIText()
        {
            armorUIText.text = $"{_stats.GetStat(MainStats.Armor)}";
        }

        private void TryDefenceSkill(float chance, float damage, GameObject instigator, SkillNames skill, MainStats stat)
        {
            var successChance = _stats.GetSkillLevel(skill) * _stats.GetStat(stat) / 100 + _stats.GetStat(MainStats.Dexterity) / 10;
            var result = Random.Range(0, chance + successChance);
            Debug.Log($"{gameObject.name} : TryDefence: Hit:{chance} SuccessChance: {successChance} / Random val: {result}");
            if (result < chance)
            {
                instigator.GetComponent<BaseStats>().SkillGain(instigator.GetComponent<Fighter>().GetWeaponSkill());
                GetComponent<Health>().TakeDamage(damage - _stats.GetStat(MainStats.Armor));
                Debug.Log($"{gameObject.name}: Damage: {damage} Armor: {_stats.GetStat(MainStats.Armor)}");
            }
            else
            {
                _stats.SkillGain(skill);
                _fighter.Defend(skill);
            }
        }

        public IEnumerable<float> GetAdditiveModifier(MainStats stat)
        {
            if (stat == MainStats.Armor) yield return armor;
            yield return 0;
        }

        public IEnumerable<float> GetPercentageModifier(MainStats stat)
        {
            yield return 0;
        }
    }
}
