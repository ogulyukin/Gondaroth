using UnityEngine;

namespace RPG.Core
{
    public enum UnitTypes
    {
        Elves,
        DarkElves,
        Monster,
        Animal,
        Mount
    }

    public enum Spells
    {
        MagicFog,
        Heal,
        FireArrow
    }
    public class ActionScheduler : MonoBehaviour
    {
        private IAction _currentAction;
        public void StartAction(IAction action)
        {
            if(_currentAction == action) return;
            if (!ReferenceEquals(_currentAction, null))
            {
                _currentAction.Cancel();
            }
            _currentAction = action;
        }

        public void CancelCurrentAction()
        {
            StartAction(null);
        }
    }
}
