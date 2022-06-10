using System;
using System.Collections.Generic;
using RPG.Inventories;
using UnityEngine;

namespace RPG.Dialogue
{
    [System.Serializable]
    public class Objective
    {
        public string reference;
        public string description;
        public int order;

        public Objective(string description)
        {
            this.description = description;
            reference = Guid.NewGuid().ToString();
            order = 0;
        }

        public Objective()
        {
            description = string.Empty;
            reference = Guid.NewGuid().ToString();
            order = 0;
        }
    }
    
    [CreateAssetMenu(fileName = "Quest", menuName = "Quest", order = 0)]
    public class Quest : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField] private string questName;
        [SerializeField] private Objective[] objectives;
        [SerializeField] private List<Reward> rewards = new List<Reward>();

        [System.Serializable]
        public class Reward
        {
            [Min(1)] public int number;
            public InventoryItem item;
        }
        public string GetTitle()
        {
            return questName;
        }

        public IEnumerable<Objective> GetObjectives()
        {
            return objectives;
        }

        public IEnumerable<Reward> GetRewardList()
        {
            return rewards;
        }

        public int GetObjectivesCount()
        {
            return objectives.Length;
        }

        public bool HasObjective(string objectiveReference)
        {
            return GetObjectiveByRef(objectiveReference) != null ? true : false;
        }

        public static Quest GetQuestByName(string questName)
        {
            foreach (var quest in Resources.LoadAll<Quest>(""))
            {
                if (quest.name == questName)
                {
                    return quest;
                }
            }

            return null;
        }
        
        public static Quest GetQuestByQuestTitle(string questName)
        {
            foreach (var quest in Resources.LoadAll<Quest>(""))
            {
                if (quest.questName == questName)
                {
                    return quest;
                }
            }
            return null;
        }

        public Objective GetObjectiveByRef(string reference)
        {
            foreach (var objective in objectives)
            {
                if (objective.reference == reference) return objective;
            }

            return null;
        }
        
        private void ObjectiveCheck()
        {
            foreach (var objective in objectives)
            {
                if (objective.reference == string.Empty) objective.reference = Guid.NewGuid().ToString();
            }
        }

        public void OnBeforeSerialize()
        {
            ObjectiveCheck();
        }

        public void OnAfterDeserialize()
        {
            
        }
    }
}
