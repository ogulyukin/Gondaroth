using UnityEngine;

namespace RPG.Dialogue
{
    public class QuestCompletion : MonoBehaviour
    {
        [SerializeField] private Quest quest;
        [SerializeField] private string objective;

        public void CompleteQuest()
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<QuestList>().CompleteObjective(quest, quest.GetObjectiveByRef(objective));
        }
    }
}
