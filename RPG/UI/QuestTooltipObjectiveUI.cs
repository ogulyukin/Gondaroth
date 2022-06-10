using TMPro;
using UnityEngine;

namespace RPG.UI
{
    public class QuestTooltipObjectiveUI : MonoBehaviour
    {
        [SerializeField] private GameObject objectiveCheck;
        [SerializeField] private TextMeshProUGUI text;
        private void Awake()
        {
            objectiveCheck.SetActive(false);
        }

        public void ToggleObjectiveCheck(bool completed)
        {
            objectiveCheck.SetActive(completed);
        }

        public void Setup(string objectiveText)
        {
            text.text = objectiveText;
        }
    }
}
