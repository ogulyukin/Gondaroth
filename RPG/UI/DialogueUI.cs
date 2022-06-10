using RPG.Dialogue;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI
{
    public class DialogueUI : MonoBehaviour
    {
        private PlayerConversant _playerConversant;
        [SerializeField] private TextMeshProUGUI aiText;
        [SerializeField] private Button nextButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private Transform choiceRoot;
        [SerializeField] private GameObject aiResponse;
        [SerializeField] private GameObject choicePrefab;
        [SerializeField] private TextMeshProUGUI conversantName;

        private void Start()
        {
            _playerConversant = GameObject.FindWithTag("Player").GetComponent<PlayerConversant>();
            UpdateUI();
            nextButton.onClick.AddListener(() => _playerConversant.Next());
            quitButton.onClick.AddListener(() => _playerConversant.QuitDialogue());
            _playerConversant.OnConversationUpdated += UpdateUI;
        }

        private void UpdateUI()
        {
            gameObject.SetActive(_playerConversant.IsActiveCurrently());
            if(!_playerConversant.IsActiveCurrently()) return;
            conversantName.text = _playerConversant.GetCurrentConversantName();
            aiResponse.SetActive(!_playerConversant.IsChoosing());
            choiceRoot.gameObject.SetActive(_playerConversant.IsChoosing());
            if (_playerConversant.IsChoosing())
            {
                UpdatePlayerDialogue();
            }
            else
            {
                aiText.text = _playerConversant.GetText();
                nextButton.gameObject.SetActive(_playerConversant.HasNext());    
            }
            
        }

        private void UpdatePlayerDialogue()
        {
            foreach (Transform item in choiceRoot)
            {
                Destroy(item.gameObject);
            }

            foreach (var choice in _playerConversant.GetChoices())
            {
                var instance = Instantiate(choicePrefab, choiceRoot);
                instance.GetComponentInChildren<TextMeshProUGUI>().text = choice.GetText();
                var button = instance.GetComponentInChildren<Button>();
                button.onClick.AddListener(() =>
                {
                    _playerConversant.MadeChoice(choice);
                });
            }
        }
    }
}
