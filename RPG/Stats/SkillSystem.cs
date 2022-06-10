using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace RPG.Stats
{
    public enum SkillNames
    {
        //Combat skills:
        Swordsmanship,
        Polearms,
        Bow,
        Shield,
        Unarmed,
        //Defense skills:
        Parring,
        Block,
        Evade,
        //Magic skills:
        Destruction,
        Illusion,
        Conjuration,
        Enchantment,
        Restoration,
        ResistMagic,
        //Survival skills:
        Camping,
        Mapping,
        Tracking,
        Healing,
        //Lore skills:
        ArmorLore,
        AnimalLore,
        Anatomy,
        DragonsLore,
        AnimalTaming,
        //Gathering skills:
        Mining,
        Lumberjack,
        Herbary,
        Farming
        //Crafting skills:
        //TODO add crafting skills here
    }

    public enum SkillGrowStatus
    {
        Normal,
        Locked,
        Relese
    }

    [System.Serializable]
    public class Skill
    {
        public SkillNames skillName;
        public MainStats dependStat;
        public string skillNameRUS;
        public bool isUsable = false;
        public SkillGrowStatus status = SkillGrowStatus.Normal;
    }
    [CreateAssetMenu(fileName = "SkillSystem", menuName = "RPG/Skillsystem", order = 0)]
    public class SkillSystem :ScriptableObject
    {
        [SerializeField] private List<Skill> skills = new List<Skill>();

        public Skill GetSkill(SkillNames skillName)
        {
            foreach (var skill in skills)
            {
                if (skill.skillName == skillName) return skill;
            }

            return null;
        }

        /*
            new Skill()
            {
                name = SkillNames.Swordsmanship,
                dependStat = MainStats.Strength,
                skillNameRUS = "Владение мечём"
            },
            new Skill()
            {
                name = SkillNames.Polearms,
                dependStat = MainStats.Dexterity,
                skillNameRUS = "Владение копьём"
            },
            new Skill()
            {
                name = SkillNames.Shield,
                dependStat = MainStats.Strength,
                skillNameRUS = "Владение Щитом"
            },
            new Skill()
            {
                name = SkillNames.Bow,
                dependStat = MainStats.Dexterity,
                skillNameRUS = "Владение Луком"
            },
            new Skill()
            {
                name = SkillNames.Unarmed,
                dependStat = MainStats.Strength,
                skillNameRUS = "Руклпашный бой"
            },
            new Skill()
            {
                name = SkillNames.Block,
                dependStat = MainStats.Strength,
                skillNameRUS = "Блок"
            },
            new Skill()
            {
                name = SkillNames.Evade,
                dependStat = MainStats.Dexterity,
                skillNameRUS = "Уклонение"
            },
            new Skill()
            {
                name = SkillNames.Parring,
                dependStat = MainStats.Dexterity,
                skillNameRUS = "Парирование ударов"
            },
            new Skill()
            {
                name = SkillNames.Anatomy,
                dependStat = MainStats.Intellect,
                skillNameRUS = "Анатомия"
            },
            new Skill()
            {
                name = SkillNames.ArmorLore,
                dependStat = MainStats.Intellect,
                skillNameRUS = "Знание Оружия и Доспехов"
            },
            new Skill()
            {
                name = SkillNames.DragonsLore,
                dependStat = MainStats.Intellect,
                skillNameRUS = "Знание Драконов"
            },
            new Skill()
            {
                name = SkillNames.AnimalLore,
                dependStat = MainStats.Intellect,
                skillNameRUS = "Знание о Животных"
            }*/
    }
}
