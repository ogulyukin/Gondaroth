using RPG.Attributes;
using RPG.Control;
using UnityEngine;

namespace RPG.Dialogue
{
    public class AIConversant : MonoBehaviour, IRaycastable
    {
        [SerializeField] private global::RPG.Dialogue.Dialogue dialogue;
        [SerializeField] private string conversantName = "Guard Name";
        public CursorType GetCursorType()
        {
            if (!GetComponent<Health>().IsAlive()) return CursorType.None;
            return CursorType.Dialogue;
        }

        public string GetConversantName()
        {
            return conversantName;
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if (dialogue == null || !GetComponent<Health>().IsAlive()) return false;
            if (Input.GetMouseButtonDown(0))
            {
                callingController.GetComponent<PlayerConversant>()?.StartNewDialog(this, dialogue);
            }
            return true;
        }
    }
}
