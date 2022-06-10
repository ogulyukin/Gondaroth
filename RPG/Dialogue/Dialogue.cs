using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RPG.Dialogue
{
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue", order = 0)]
    public class Dialogue : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField] private List<DialogueNode> nodes = new List<DialogueNode>();
        [SerializeField] private Texture2D background;
        [SerializeField] private float dialogCanvasSize = 2000f;
        
        private Dictionary<string, DialogueNode> _nodeLookup = new Dictionary<string, DialogueNode>();
        private void OnValidate()
        {
            _nodeLookup.Clear();
            FillNodeLookup();
        }

        public Texture2D GetBackground()
        {
            return background;
        }

        public float GetDialogCanvasSize()
        {
            return dialogCanvasSize;
        }
        private void FillNodeLookup()
        {
            foreach (var node in GetAllNodes())
            {
                _nodeLookup.Add(node.name, node);
            }
        }

        public IEnumerable<DialogueNode> GetAllNodes()
        {
            return nodes;
        }

        public DialogueNode GetRootNode()
        {
            return nodes[0];
        }

        public IEnumerable<DialogueNode> GetAllChildren(DialogueNode dialogNode)
        {
            foreach (var child in dialogNode.GetChildren())
            {
                if (_nodeLookup.ContainsKey(child)) yield return _nodeLookup[child];
            }
        }
        
#if UNITY_EDITOR
        public void CreateNode(DialogueNode parentNode)
        {
            var newNode = MakeDialogueNode(parentNode);
            Undo.RegisterCreatedObjectUndo(newNode, "Creating new Dialogue Node");
            Undo.RegisterCreatedObjectUndo(this, "Added new Dialogue Node");
            newNode.SetSpeaker(!parentNode.GetSpeaker());
            newNode.SetPosition(new Vector2(parentNode.GetRect().xMax + 20, parentNode.GetRect().y));
            AddNode(newNode);
        }
        private void AddNode(DialogueNode newNode)
        {
            nodes.Add(newNode);
            _nodeLookup.Add(newNode.name, newNode);
        }

        private static DialogueNode MakeDialogueNode(DialogueNode parentNode)
        {
            var newNode = CreateInstance<DialogueNode>();
            newNode.name = Guid.NewGuid().ToString();
            if (parentNode != null) parentNode.AddChild(newNode.name);
            return newNode;
        }
        public void DeleteNode(DialogueNode nodeToDelete)
        {
            Undo.RecordObject(this, "Deleting Dialog Node");
            nodes.Remove(nodeToDelete);
            OnValidate();
            CleanDanglingChildren(nodeToDelete);
            Undo.DestroyObjectImmediate(nodeToDelete);
        }

        private void CleanDanglingChildren(DialogueNode nodeToDelete)
        {
            foreach (var node in GetAllNodes())
            {
                node.RemoveChild(nodeToDelete.name);
            }
        }
#endif
        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if (nodes.Count == 0)
            {
                var newNode = MakeDialogueNode(null);
                AddNode(newNode);
            }
            
            if (AssetDatabase.GetAssetPath(this) != string.Empty)
            {
                foreach (var node in GetAllNodes())
                {
                    if(AssetDatabase.GetAssetPath(node) == string.Empty) AssetDatabase.AddObjectToAsset(node, this);
                }
            }
#endif             
        }

        public void OnAfterDeserialize()
        {
            //nothing to do here
        }
    }
}
