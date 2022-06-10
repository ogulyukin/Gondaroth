using System;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace RPG.Dialogue.Editor
{
    public class DialogueEditor : EditorWindow
    {
        private global::RPG.Dialogue.Dialogue _selectedDialogue;
        [NonSerialized] private GUIStyle _nodeStyle;
        [NonSerialized] private GUIStyle _playerNodeStyle;
        [NonSerialized] private DialogueNode _draggingNode;
        [NonSerialized] private Vector2 _draggingOffset;
        [NonSerialized] private DialogueNode _creatingNode;
        [NonSerialized] private DialogueNode _deletingNode;
        [NonSerialized] private DialogueNode _linkingParentNode;
        [NonSerialized] private bool _draggingCanvas;
        [NonSerialized] private Vector2 _draggingCanvasOffset;
        private Vector2 _scrollPosition;
        private const float BackgroundSize = 50f;
        
        [MenuItem("Window/Dialogue Editor")]
        public static void  ShowEditorWindow()
        {
            GetWindow(typeof(DialogueEditor), false, "Dialogue Editor");
        }

        private void OnEnable()
        {
            Selection.selectionChanged += OnSelectionChanged;
            _nodeStyle = new GUIStyle
            {
                normal =
                {
                    background = EditorGUIUtility.Load("node0") as Texture2D,
                    textColor = Color.white
                },
                padding = new RectOffset(20,20,20,20),
                border = new RectOffset(12,12,12,12)
            };
            _playerNodeStyle = new GUIStyle
            {
                normal =
                {
                    background = EditorGUIUtility.Load("node1") as Texture2D,
                    textColor = Color.white
                },
                padding = new RectOffset(20,20,20,20),
                border = new RectOffset(12,12,12,12)
            };
        }

        private void OnDisable()
        {
            Selection.selectionChanged -= OnSelectionChanged;
        }

        [OnOpenAsset(1)]
        public static bool OpenDialogue(int instanceID, int line)
        {
            var dialogue = EditorUtility.InstanceIDToObject(instanceID) as global::RPG.Dialogue.Dialogue;
            if(dialogue == null) return false;
            ShowEditorWindow();
            return true;
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField(_selectedDialogue != null ? _selectedDialogue.name : "No Dialogue selected");
            
            if (_selectedDialogue != null)
            {
                ProcessEvent();
                _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
                var canvasSize = _selectedDialogue.GetDialogCanvasSize();
                var canvas = GUILayoutUtility.GetRect(canvasSize, canvasSize);
                GUI.DrawTextureWithTexCoords(canvas,_selectedDialogue.GetBackground(), new Rect(0, 0 , canvasSize / BackgroundSize,
                    canvasSize / BackgroundSize));
                
                foreach (var dialogNode in _selectedDialogue.GetAllNodes())
                {
                    DrawConnections(dialogNode);
                }
                foreach (var dialogNode in _selectedDialogue.GetAllNodes())
                {
                    DrawNode(dialogNode);
                }
                
                EditorGUILayout.EndScrollView();

                ManageNodes();
            }

           
        }

        private void ManageNodes()
        {
            if (_creatingNode != null)
            {
                _selectedDialogue.CreateNode(_creatingNode);
                _creatingNode = null;
            }

            if (_deletingNode != null)
            {
                _selectedDialogue.DeleteNode(_deletingNode);
                _deletingNode = null;
            }
        }

        private void ProcessEvent()
        {
            if (_draggingNode == null && Event.current.type == EventType.MouseDown)
            {
                _draggingNode = GetNodeAtPoint(Event.current.mousePosition + _scrollPosition);
                if (_draggingNode != null)
                {
                    _draggingOffset = _draggingNode.GetRect().position - (Event.current.mousePosition);
                    Selection.activeObject = _draggingNode;
                }
                else
                {
                    _draggingCanvas = true;
                    _draggingCanvasOffset = Event.current.mousePosition + _scrollPosition;
                    Selection.activeObject = _selectedDialogue;
                }
            }else if (_draggingNode != null && Event.current.type == EventType.MouseDrag)
            {
                _draggingNode.SetPosition(Event.current.mousePosition + _draggingOffset);
                GUI.changed = true;
            }else if (_draggingCanvas && Event.current.type == EventType.MouseDrag)
            {
                _scrollPosition = _draggingCanvasOffset - Event.current.mousePosition;
                GUI.changed = true;
            }

            if (Event.current.type == EventType.MouseUp)
            {
                _draggingNode = null;
                _draggingCanvas = false;
            }
        }

        private DialogueNode GetNodeAtPoint(Vector2 point)
        {
            DialogueNode foundNode = null;
            foreach (var node in _selectedDialogue.GetAllNodes())
            {
                if (node.GetRect().Contains(point)) foundNode = node;
            }

            return foundNode;
        }

        private void DrawNode(DialogueNode dialogNode)
        {
            GUILayout.BeginArea(dialogNode.GetRect(), dialogNode.GetSpeaker() ? _playerNodeStyle : _nodeStyle);
            dialogNode.SetText(EditorGUILayout.TextField(dialogNode.GetText()));
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add"))
            {
                _creatingNode = dialogNode;
            }

            DrawLinksButtons(dialogNode);

            if (GUILayout.Button("Delete"))
            {
                _deletingNode = dialogNode;
            }
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        private void DrawLinksButtons(DialogueNode dialogNode)
        {
            if (_linkingParentNode == null)
            {
                if (GUILayout.Button("Link"))
                {
                    _linkingParentNode = dialogNode;
                }
            }
            else if (_linkingParentNode == dialogNode)
            {
                if (GUILayout.Button("Cansel"))
                {
                    _linkingParentNode = null;
                }
            }
            else if (_linkingParentNode.GetChildren().Contains(dialogNode.name))
            {
                if (GUILayout.Button("Unlink"))
                {
                    _linkingParentNode.RemoveChild(dialogNode.name);
                    _linkingParentNode = null;
                }
            }else
            {
                if (GUILayout.Button("Child"))
                {
                    _linkingParentNode.AddChild(dialogNode.name);
                    _linkingParentNode = null;
                }
            }
        }

        private void DrawConnections(DialogueNode node)
        {
            var startPosition = new Vector2(node.GetRect().xMax, node.GetRect().center.y);
            foreach (var childNode in _selectedDialogue.GetAllChildren(node))
            {
                var endPosition = new Vector2(childNode.GetRect().x, childNode.GetRect().center.y);
                var controlPointOffset = new Vector2((endPosition.x - startPosition.x) * 0.8f, 0);
                Handles.DrawBezier(startPosition, endPosition, startPosition + controlPointOffset ,
                    endPosition - controlPointOffset, Color.white,  null, 4f);
            }
        }

        private void OnSelectionChanged()
        {
            var dialogue = Selection.activeObject as global::RPG.Dialogue.Dialogue;
            if (dialogue != null)
            {
                _selectedDialogue = dialogue;
                Repaint();
            }
        }
    }
}
