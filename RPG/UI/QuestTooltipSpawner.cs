using GameDevTV.Core.UI.Tooltips;
using UnityEngine;

namespace RPG.UI
{
    public class QuestTooltipSpawner : TooltipSpawner
    {
        public override void UpdateTooltip(GameObject tooltip)
        {
            tooltip.GetComponent<QuestTooltipUI>().Setup(GetComponent<QuestItemUI>().GetQuestStatus());
        }

        public override bool CanCreateTooltip()
        {
            return true;
        }
    }
}
