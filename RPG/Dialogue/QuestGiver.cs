using UnityEngine;

namespace RPG.Dialogue
{
    public class QuestGiver : MonoBehaviour
    {
        [SerializeField] private Quest quest;

        public void GiveQuest()
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<QuestList>().AddQuest(quest);
        }
    }
}
