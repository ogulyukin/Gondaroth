using System.Linq;
using System.Text;
using RPG.Dialogue;
using TMPro;
using UnityEngine;

namespace RPG.UI
{
    public class QuestTooltipUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI rewardText;
        [SerializeField] private Transform objectivesContainer;
        [SerializeField] private GameObject objectivePrefab;
        public void Setup(QuestStatus quest)
        {
            ClearObjectives();

            titleText.text = quest.GetQuest().GetTitle();
            
            foreach (var objective in quest.GetQuest().GetObjectives())
            {
                var item = Instantiate(objectivePrefab, objectivesContainer.transform);
                item.GetComponent<QuestTooltipObjectiveUI>().Setup(objective.description); 
                if(quest.IsObjectiveCompeted(objective.reference)) item.GetComponent<QuestTooltipObjectiveUI>().ToggleObjectiveCheck(true);
            }

            rewardText.text = GenerateRewardText(quest);
        }

        private void ClearObjectives()
        {
            foreach (var child in objectivesContainer.GetComponentsInChildren<QuestTooltipObjectiveUI>())
            {
                Destroy(child.gameObject);
            }
        }

        private static string GenerateRewardText(QuestStatus quest)
        {
            if (!quest.GetQuest().GetRewardList().Any()) return "No reward";
            var rewardString = new StringBuilder();
            foreach (var reward in quest.GetQuest().GetRewardList())
            {
                if (rewardString.Length > 0) rewardString.Append(", ");
                if (reward.number > 1) rewardString.Append(reward.number);
                rewardString.Append($" {reward.item.name}");
            }
            return rewardString.ToString();
        }
    }
}
    