using System;
using RPG.Core;
using RPG.Stats;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.Skills
{
    public class SkillRowItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI skillText;
        [SerializeField] private TextMeshProUGUI skillLevel;
        [SerializeField] private Button controlButton;
        [SerializeField] private Button useButton;
        [SerializeField] private Image upImage;
        [SerializeField] private Image downImage;
        [SerializeField] private Image pauseImage;
        
        private BaseStats.SkillRow  _skill;
        public void Setup(BaseStats.SkillRow skill)
        {
            _skill = skill;
            var skillSystem = GameObject.FindGameObjectWithTag("Core").GetComponent<SkillSystemCore>();
            var skillName = skillSystem.skills.GetSkill(_skill.skillName).skillNameRUS;
            skillText.text = skillName;
            skillLevel.text = $"{_skill.skillLevel}";
            //Debug.Log($"{skillName} : {skill.skillLevel}");
            if (!skillSystem.skills.GetSkill(_skill.skillName).isUsable) useButton.interactable = false;
            ChangeSkillStatus();
        }

        public void  ToggleSkillStatus()
        {
            var control = _skill.status;
            switch (control)
            {
                case SkillGrowStatus.Normal:
                    _skill.status = SkillGrowStatus.Locked;
                    ChangeSkillStatus();
                    break;
                case SkillGrowStatus.Locked:
                    _skill.status = SkillGrowStatus.Relese;
                    ChangeSkillStatus();
                    break;
                case SkillGrowStatus.Relese:
                    _skill.status = SkillGrowStatus.Normal;
                    ChangeSkillStatus();
                    break;
            }
        }
        private void ChangeSkillStatus()
        {
            var control = _skill.status;
            switch (control)
            {
                case SkillGrowStatus.Normal:
                    ToggleOffImages();
                    upImage.enabled = true;
                    break;
                case SkillGrowStatus.Locked:
                    ToggleOffImages();
                    pauseImage.enabled = true;
                    break;
                case SkillGrowStatus.Relese:
                    ToggleOffImages();
                    downImage.enabled = true;
                    break;
            }
        }

        private void ToggleOffImages()
        {
            upImage.enabled = false;
            downImage.enabled = false;
            pauseImage.enabled = false;
        }
    }
}
