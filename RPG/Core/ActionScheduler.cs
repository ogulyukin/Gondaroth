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

    public enum UnitStates
    {
        Moving,
        Fighting,
        UsingSkill,
        Iddle,
        Confusing,
        Blocking,
        Dodging,
        GotHit
    }

    public enum Spells
    {
        MagicFog,
        Heal,
        FireArrow
    }
    public class ActionScheduler : MonoBehaviour
    {
        private IAction currentAction;
        public void StartAction(IAction action)
        {
            if(currentAction == action) return;
            if (currentAction != null)
            {
                currentAction.Cancel();
            }
            currentAction = action;
        }

        public void CancelCurrentAction()
        {
            StartAction(null);
        }
    }
}
