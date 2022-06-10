using System.Collections.Generic;
using RPG.Core;
using UnityEditor;
using UnityEngine;

namespace RPG.Dialogue
{
    public class DialogueNode : ScriptableObject
    {
        [SerializeField] private bool isPlayerSpeaking;
        [SerializeField] private string text;
        [SerializeField] private List<string> children = new List<string>();
        [SerializeField] private Rect rectPosition = new Rect(5,5, 200,100);
        [SerializeField] private string onEnterAction;
        [SerializeField] private string onExitAction;
        [SerializeField] private Condition condition;

        public bool CheckCondition(IEnumerable<IPredicateEvaluator> evaluators)
        {
            return condition.Check(evaluators);
        }
        public Rect GetRect()
        {
            return rectPosition;
        }
        public string GetText()
        {
            return text;
        }
        
        public List<string> GetChildren()
        {
            return children;
        }

        public bool GetSpeaker()
        {
            return isPlayerSpeaking;
        }

        public string GetOnEnterAction()
        {
            return onEnterAction;
        }

        public string GetOnExitAction()
        {
            return onExitAction;
        }
#if UNITY_EDITOR
        public void SetText(string newText)
        {
            if (newText != text)
            {
                Undo.RecordObject(this, "Update Dialogue Text");
                text = newText;    
                EditorUtility.SetDirty(this);
            }
        }
        
        public void SetPosition(Vector2 position)
        {
            Undo.RecordObject(this, "Update Dialogue Node Position");
            rectPosition.position = position;
            EditorUtility.SetDirty(this);
        }

        public void AddChild(string newChild)
        {
            Undo.RecordObject(this, "Add Linked Dialog Node");
            children.Add(newChild);
            EditorUtility.SetDirty(this);
        }

        public void RemoveChild(string child)
        {
            Undo.RecordObject(this, "Unlink Dialog Node");
            children.Remove(child);
            EditorUtility.SetDirty(this);
        }

        public void SetSpeaker(bool speaker)
        {
            Undo.RecordObject(this, "Set Speaker mode");
            isPlayerSpeaking = speaker;
            EditorUtility.SetDirty(this);
        }
#endif
    }
}
