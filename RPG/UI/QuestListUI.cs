using RPG.Dialogue;
using UnityEngine;

namespace RPG.UI
{
    public class QuestListUI : MonoBehaviour
    {
        [SerializeField] private QuestItemUI questPrefab;

        private void Start()
        {
            UpdateQuestListUI();
        }

        private void OnEnable()
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<QuestList>().UpdateQuestList += UpdateQuestListUI;
        }
        
        private void UpdateQuestListUI()
        {
            foreach (var child in transform.GetComponentsInChildren<QuestItemUI>())
            {
                Destroy(child.gameObject);
            }

            foreach (var quest in (GameObject.FindGameObjectWithTag("Player")).GetComponent<QuestList>()
                     .GetPlayerQuestsStatuses())
            {
                var instance = Instantiate(questPrefab, transform);
                instance.Setup(quest);
            }
        }
    }
}
