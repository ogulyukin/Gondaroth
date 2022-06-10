using RPG.Dialogue;
using TMPro;
using UnityEngine;

namespace RPG.UI
{
    public class QuestItemUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI progress;
        private QuestStatus _quest;
        public void Setup(QuestStatus quest)
        {
            _quest = quest;
            title.text = quest.GetQuest().GetTitle();
            progress.text = $"{quest.GetCompletedObjectivesCount()}/{quest.GetQuest().GetObjectivesCount()}";
        }

        public QuestStatus GetQuestStatus()
        {
            return _quest;
        }
    }
}
