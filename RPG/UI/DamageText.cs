using System;
using RPG.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI
{
    public class DamageText : MonoBehaviour
    {
        public void DestroyDamageText()
        {
            Destroy(gameObject);
        }
    }
}
