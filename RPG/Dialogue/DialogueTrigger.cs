using UnityEngine;
using UnityEngine.Events;

namespace RPG.Dialogue
{
    [System.Serializable]
    public class DialogueTriggerData
    {
        public string action;
        public UnityEvent onTrigger;
    }
    public class DialogueTrigger : MonoBehaviour
    {
        [SerializeField] private DialogueTriggerData[] actions;
           
        public void Trigger(string actionToTrigger)
        {
            foreach (var action in actions)
            {
                if(action.action == actionToTrigger) action.onTrigger?.Invoke();    
            }
        }
    }
}
