using System;
using RPG.Stats;
using UnityEngine;

namespace RPG.UI.Skills
{
    public class SkillList : MonoBehaviour
    {
        [SerializeField] private GameObject skillItemPrefab;

        private void Start()
        {
            UpdateSkillListUI();
        }

        private void OnEnable()
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<BaseStats>().UpdateSkillList += UpdateSkillListUI;
        }

        private void UpdateSkillListUI()
        {
            foreach (var item in transform.GetComponentsInChildren<SkillRowItem>())
            {
                Destroy(item.gameObject);
            }

            foreach (var skill in GameObject.FindGameObjectWithTag("Player").GetComponent<BaseStats>().GetSkillList())
            {
                var skillRow = Instantiate(skillItemPrefab, transform);
                skillRow.GetComponent<SkillRowItem>().Setup(skill);
            }
            
        }
    }
}
