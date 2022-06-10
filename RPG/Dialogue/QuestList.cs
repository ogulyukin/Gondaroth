using System;
using System.Collections.Generic;
using RPG.Core;
using RPG.Inventories;
using Saving;
using UnityEngine;

namespace RPG.Dialogue
{
    public class QuestList : MonoBehaviour, ISaveable, IPredicateEvaluator
    {
        private List<QuestStatus> _statuses;
        public event Action UpdateQuestList;

        private void Awake()
        {
            _statuses = new List<QuestStatus>();
        }

        public IEnumerable<QuestStatus> GetPlayerQuestsStatuses()
        {
            return _statuses;
        }

        public void AddQuest(Quest quest)
        {
            if (HasQuest(quest)) return;
            _statuses.Add(new QuestStatus(quest));
            UpdateQuestList?.Invoke();
        }

        public void CompleteObjective(Quest quest, Objective objective)
        {
            if (!ValidateObjective(quest, objective)) return;
            foreach (var status in _statuses)
            {
                if (status.GetQuest() == quest)
                {
                    Debug.Log("Quest found! in QuestList");
                    if (!status.HasCompletedAlready(objective.reference))
                    {
                        status.CompeteObjective(objective.reference);
                        Debug.Log($"Objective {objective.description} completed in quest {quest.GetTitle()}");
                        if (status.IsQuestComplete())
                        {
                            GiveReward(quest);
                        }
                        UpdateQuestList?.Invoke();
                    }
                    return;
                }
            }
            Debug.Log($"Quest {quest.GetTitle()} not found!");
        }

        private void GiveReward(Quest quest)
        {
            foreach (var reward in quest.GetRewardList())
            {
                if (!GetComponent<Inventory>().AddToFirstEmptySlot(reward.item, reward.number))
                {
                    GetComponent<ItemDropper>().DropItem(reward.item,reward.number);
                }
            }
        }
        private static bool ValidateObjective(Quest quest, Objective objective)
        {
            if (!quest.HasObjective(objective.reference))
            {
                Debug.Log($"Objective {objective.description} not found in quest {quest.GetTitle()}");
                return false;
            }
            Debug.Log($"Objective {objective.description} is present in quest {quest.GetTitle()}");
            return true;
        }

        private bool HasQuest(Quest quest)
        {
            Debug.Log($"HasQuest param: {quest.name}");
            foreach (var status in _statuses)
            {
                Debug.Log($"HasQuest iteration: {status.GetQuest().name}");
                if (status.GetQuest() == quest) return true;
            }

            return false;
        }

        public object CaptureState()
        {
            List<object> state = new List<object>();
            foreach (var status in _statuses)
            {
                state.Add(status.CaptureState());
            }
            return state;
        }

        public void RestoreState(object state)
        {
            var stateList = state as List<object>;
            if(stateList == null) return;
            _statuses.Clear();
            foreach (var item in stateList)
            {
                _statuses.Add(new QuestStatus(item));
            }
            UpdateQuestList?.Invoke();
        }

        public bool? Evaluate(string predicate, string[] parameters)
        {
            switch (predicate)
            {
                case "DontHasQuest":
                    return !HasQuest(Quest.GetQuestByQuestTitle(parameters[0]));
                case "HasQuest":
                    return HasQuest(Quest.GetQuestByQuestTitle(parameters[0]));
                case "CompletedQuest":
                    return GetQuestsStatus(Quest.GetQuestByQuestTitle(parameters[0]));
                default:
                    return null;
            }
        }

        private bool? GetQuestsStatus(Quest quest)
        {
            return quest.GetObjectivesCount() == GetCompletedObjectivesInQuest(quest);
        }

        private int GetCompletedObjectivesInQuest(Quest quest)
        {
            foreach (var status in _statuses)
            {
                if (status.GetQuest() == quest) return status.GetCompletedObjectivesCount();
            }

            return 0;
        }
    }
}
