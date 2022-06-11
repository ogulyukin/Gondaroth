using System;
using System.Collections.Generic;
using RPG.Core;
using RPG.Inventories;
using Saving;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace RPG.Stats
{
    [System.Serializable]
    public class StatUnit
    {
        public MainStats statName;
        public float statLevel;
    }
    
    public enum MainStats
    {
        Strength,
        Intellect,
        Dexterity,
        Attack,
        Parry,
        Block,
        Evade,
        Damage,
        Armor,
        None
    }
    public class BaseStats : MonoBehaviour, ISaveable
    {
        //New Skill System
        [SerializeField] private int MaxSummStats = 225;
        [SerializeField] private List<StatUnit> stats = new List<StatUnit>();
        [SerializeField] private List<SkillRow> skills = new List<SkillRow>();
        [SerializeField] private int dropLevel = 1;
        [SerializeField] private bool isStaticNPC = true;
        private SkillSystemCore _skillSystem;

        public event Action UpdateSkillList;
        
        private Dictionary<SkillNames,SkillRow > _skills;

        [System.Serializable]
        public class SkillRow
        {
            public SkillNames skillName;
            public float skillLevel;
            public float maxLevel = 100;
            public SkillGrowStatus status = SkillGrowStatus.Normal;
        }

        private void Start()
        {
            _skillSystem = GameObject.FindGameObjectWithTag("Core").GetComponent<SkillSystemCore>();
            _skills = new Dictionary<SkillNames, SkillRow>();
            foreach (var skill in skills)
            {
                _skills.Add(skill.skillName, skill);
            }
            UpdateMainStats();
            var equipment = GetComponent<Equipment>();
            if(equipment != null) equipment.EquipmentUpdated += UpdateMainStats;
        }

        public bool IsNpc()
        {
            return isStaticNPC;
        }
        public IEnumerable<SkillRow> GetSkillList()
        {
            return skills;
        }
        public void SkillGain(SkillNames skill)
        {
            if(isStaticNPC) return;
            CheckIsExistSkill(skill);
            if (Random.Range(0, 100) > _skills[skill].skillLevel)
            {
                _skills[skill].skillLevel = Mathf.Min(_skills[skill].skillLevel + 0.1f, _skills[skill].maxLevel);
                UpdateSkillInList(skill);
                StatGain(skill);
                UpdateSkillList?.Invoke();
            }
            Debug.Log($"{gameObject.name} : {skill} : { _skills[skill].skillLevel}");
        }

        private void CheckIsExistSkill(SkillNames skill)
        {
            if (_skills.ContainsKey(skill)) return;
            
            Debug.Log($"Добавляю: {skill} : {_skills.ContainsKey(skill)}");
            var newSkill = new SkillRow()
            {
                skillLevel = 0,
                skillName = skill,
                status = SkillGrowStatus.Normal
            };
            _skills.Add(skill, newSkill);
            skills.Add(newSkill);
        }

        private void StatGain(SkillNames skillName)
        {
            if(isStaticNPC) return;
            float totalStatPoints = 0;
            foreach (var item in stats)
            {
                if (item.statName == MainStats.Strength || item.statName == MainStats.Dexterity ||
                    item.statName == MainStats.Intellect) totalStatPoints += item.statLevel;
            }
            if(totalStatPoints > MaxSummStats) return;
            var stat = _skillSystem.skills.GetSkill(skillName).dependStat;
            foreach (var item in stats)
            {
                if (item.statName == stat)
                {
                    item.statLevel += 1;
                    return;
                }
            }
        }
        
        public float GetSkillLevel(SkillNames skillName)
        {
            return _skills.ContainsKey(skillName) ? _skills[skillName].skillLevel : 0;
        }
        public float GetStat(MainStats stat)
        {
            var percentage = GetPercentageModifier(stat);
            if(stat == MainStats.Strength || stat == MainStats.Dexterity || stat == MainStats.Intellect)
                return (GetBaseStat(stat)  + GetAdditiveModifier(stat) * (Mathf.Round(percentage)  == 1 ? 0 : percentage));
            return (GetBaseStat(stat));
        }

        public int GetDropLevel()
        {
            return dropLevel;
        }

        private void UpdateMainStats()
        {
            UpdateStat(MainStats.Armor, CalculateStatLevel(MainStats.Armor));
            UpdateStat(MainStats.Attack, CalculateStatLevel(MainStats.Attack));
            UpdateStat(MainStats.Block, CalculateStatLevel(MainStats.Block));
            UpdateStat(MainStats.Parry, CalculateStatLevel(MainStats.Parry));
            UpdateStat(MainStats.Damage, CalculateStatLevel(MainStats.Damage));
            UpdateStat(MainStats.Evade, CalculateStatLevel(MainStats.Evade));
        }

        private float CalculateStatLevel(MainStats stat)
        {
            var statLevel = GetAdditiveModifier(stat);
            var percentage = GetPercentageModifier(stat);
            if(Mathf.Round(percentage) != 1) statLevel += statLevel * percentage;
            return statLevel;
        }
        private void UpdateStat(MainStats stat, float level)
        {
            foreach (var it in stats)
            {
                if (it.statName == stat)
                {
                    it.statLevel = level;
                    return;
                }
            }
            stats.Add(new StatUnit()
            {
                statName = stat,
                statLevel = level
            });
        }

        private void UpdateSkillInList(SkillNames skillName)
        {
            for (var i = 0; i < skills.Count; i++)
            {
                //Debug.Log($"Проверка skills {skillName} : {skills[i].skillName}");
                if (skills[i].skillName == skillName)
                {
                    skills[i].skillLevel = _skills[skillName].skillLevel;
                    return;
                }
            }
        }
        
        private float GetAdditiveModifier(MainStats stat)
        {
            var total = 0f;
            foreach (var provider in GetComponents<IModifierProvider>())
            {
                foreach (var modifier in provider.GetAdditiveModifier(stat))
                {
                    total += modifier;
                }
            }
            return total;
        }
        private float GetBaseStat(MainStats stat)
        {
            foreach (var unit in stats)
            {
                if (unit.statName == stat) return unit.statLevel;
            }

            return 0;
        }
        
        private float GetPercentageModifier(MainStats stat)
        {
            var total = 0f;
            foreach (var provider in GetComponents<IModifierProvider>())
            {
                foreach (var modifier in provider.GetPercentageModifier(stat))
                {
                    total += modifier;
                }
            }
            return total == 0 ? 1 : 1 + total / 100;
        }

        public object CaptureState()
        {
            var data = new Dictionary<string, object>();
            foreach (var item in stats)
            {
                data.Add(item.statName.ToString(), item.statLevel);
            }

            foreach (var item in skills)
            {
                data.Add(item.skillName.ToString(), item);
            }
            
            return data;
        }

        public void RestoreState(object state)
        {
            if (state is Dictionary<string, object> data)
            {
                foreach (var item in stats)
                {
                    item.statLevel = (float)data[item.statName.ToString()];
                }
                skills.Clear();
                foreach (var item in data)
                {
                    var skillRow = (SkillRow)item.Value;
                    if ( skillRow != null)
                    {
                        skills.Add(skillRow);
                    }
                }
            }
        }
    }
}
