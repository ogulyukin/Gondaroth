using System.Collections.Generic;
using System.Linq;

namespace RPG.Dialogue
{
    public class QuestStatus
    {
        private Quest _quest;
        private List<string> _completedObjectives;
        
        [System.Serializable]
        private class QuestStatusRecord
        {
            public string questName;
            public List<string> completedObjectives;
        }
        public QuestStatus(Quest quest)
        {
            this._quest = quest;
            _completedObjectives = new List<string>();
        }

        public QuestStatus(object state)
        {
            if (!(state is QuestStatusRecord newStatus)) return;
            _quest = Quest.GetQuestByName(newStatus.questName);
            _completedObjectives = newStatus.completedObjectives;
        }

        public bool IsQuestComplete()
        {
            return _quest.GetObjectives().Count() == _completedObjectives.Count;
        }

        public Quest GetQuest()
        {
            return _quest;
        }

        public int GetCompletedObjectivesCount()
        {
            return _completedObjectives.Count;
        }

        public bool IsObjectiveCompeted(string objectiveRef)
        {
            return _completedObjectives.Contains(objectiveRef);
        }

        public bool HasCompletedAlready(string objectiveRef)
        {
            return _completedObjectives.Contains(objectiveRef);
        }

        public void CompeteObjective(string objectiveRef)
        {
            _completedObjectives.Add(objectiveRef);
        }

        public object CaptureState()
        {
            return new QuestStatusRecord()
            {
                questName = _quest.name,
                completedObjectives = _completedObjectives
            };
        }
    }
}
