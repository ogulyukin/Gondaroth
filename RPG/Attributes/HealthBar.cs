using System;
using UnityEngine;

namespace RPG.Attributes
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private Health heathComponent;
        [SerializeField] private RectTransform foreground;
        [SerializeField] private Canvas rootCanvas;
        private void Update()
        {
            if (Mathf.Approximately(heathComponent.GetHeathFraction(), 0) || Mathf.Approximately(heathComponent.GetHeathFraction(), 1))
            {
                rootCanvas.enabled = false;
                return;
            }

            rootCanvas.enabled = true;
            foreground.localScale = new Vector3(heathComponent.GetHeathFraction(), 1, 1);
        }
    }
}
