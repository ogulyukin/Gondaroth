using System;
using System.Collections.Generic;
using System.Linq;
using RPG.Core;
using UnityEngine;
using Random = UnityEngine.Random;

namespace RPG.Dialogue
{
    public class PlayerConversant : MonoBehaviour
    {
        [SerializeField] private RPG.Dialogue.Dialogue currentDialogue;
        [SerializeField] private string playerName = "Player Name";
        public event Action OnConversationUpdated;

        private DialogueNode _currentNode;
        private bool _isChoosing;
        private AIConversant _currentConversant;

        public string GetCurrentConversantName()
        {
            return _isChoosing ? playerName : _currentConversant.GetConversantName(); 
        }

        public void StartNewDialog(AIConversant newConversant, RPG.Dialogue.Dialogue newDialogue)
        {
            currentDialogue = newDialogue;
            _currentConversant = newConversant;
            _currentNode = currentDialogue.GetRootNode();
            TriggerEnterAction();
            OnConversationUpdated?.Invoke();
        }

        public void QuitDialogue()
        {
            currentDialogue = null;
            TriggerExitAction();
            _isChoosing = false;
            _currentNode = null;
            _currentConversant = null;
            OnConversationUpdated?.Invoke();
        }
        public bool IsActiveCurrently()
        {
            return currentDialogue != null;
        }
        public string GetText()
        {
            if (currentDialogue != null && _currentNode == null) _currentNode = currentDialogue.GetRootNode();
            return (_currentNode == null) ? string.Empty : _currentNode.GetText();
        }

        public void Next()
        {
            var children = FilterOnCondition(currentDialogue.GetAllChildren(_currentNode)).ToArray();
            if (children[0].GetSpeaker())
            {
                _isChoosing = true;
                OnConversationUpdated?.Invoke();
                return;
            }
            TriggerExitAction();
            _currentNode = (children.Count() > 1) ? children[Random.Range(0, children.Length - 1)] : children[0];
            TriggerEnterAction();
            _isChoosing = false;
            OnConversationUpdated?.Invoke();
        }

        public void MadeChoice(DialogueNode node)
        {
            if(!currentDialogue.GetAllChildren(node).Any())
            {
                QuitDialogue();
                return;
            }
            var children = FilterOnCondition(currentDialogue.GetAllChildren(node)).ToArray();
            TriggerExitAction();
            _currentNode = (children.Count() == 1) ? children[0] : children[Random.Range(0, children.Length - 1)];
            TriggerEnterAction();
            _isChoosing = _currentNode.GetSpeaker();
            OnConversationUpdated?.Invoke();
        }

        public bool IsChoosing()
        {
            return _isChoosing;
        }

        public bool HasNext()
        {
            return FilterOnCondition(currentDialogue.GetAllChildren(_currentNode)).Any();
        }

        public IEnumerable<DialogueNode> GetChoices()
        {
            return FilterOnCondition(currentDialogue.GetAllChildren(_currentNode));
        }

        private void TriggerEnterAction()
        {
            if (_currentNode != null && _currentNode.GetOnEnterAction() != string.Empty)
            {
                TriggerAction(_currentNode.GetOnEnterAction());
                //Debug.Log(_currentNode.GetOnEnterAction());
            }
        }

        private IEnumerable<DialogueNode> FilterOnCondition(IEnumerable<DialogueNode> inputNode)
        {
            foreach (var node in inputNode)
            {
                if (node.CheckCondition(GetEvaluators())) yield return node;
            }
        }

        private IEnumerable<IPredicateEvaluator> GetEvaluators()
        {
            return GetComponents<IPredicateEvaluator>();
        }

        private void TriggerExitAction()
        {
            if (_currentNode != null && _currentNode.GetOnExitAction() != string.Empty)
            {
                TriggerAction(_currentNode.GetOnExitAction());
                //Debug.Log(_currentNode.GetOnExitAction());
            }
        }

        private void TriggerAction(string action)
        {
            foreach (var trigger in _currentConversant.GetComponents<DialogueTrigger>())
            {
                trigger.Trigger(action);
            }
        }
    }
}
