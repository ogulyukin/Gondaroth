using System;
using RPG.Stats;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Attributes
{
    public class Stamina : MonoBehaviour
    {
        [SerializeField] private Slider staminaSlider;
        [SerializeField] private TMP_Text staminaText;
        private  BaseStats _baseStat;
        private float _staminaPoints = -1f;

        private void Awake()
        {
            _baseStat = GetComponent<BaseStats>();
        }

        private void Start()
        {
            if (_staminaPoints < 0) _staminaPoints = _baseStat.GetStat(MainStats.Dexterity);
            UpdateStaminaUI();
        }

        private void Update()
        {
            var dexterity = _baseStat.GetStat(MainStats.Dexterity);
            if (dexterity > _staminaPoints)
            {
                _staminaPoints += dexterity / 100;
                UpdateStaminaUI();
            }
        }

        public float GetStaminaFraction()
        {
            return _staminaPoints/_baseStat.GetStat(MainStats.Dexterity);
        }

        public bool UseSkill(float price)
        {
            //Debug.Log($"{gameObject.name} Stamina check: {_staminaPoints}/{price}");
            if (_staminaPoints - price >= 0)
            {
                _staminaPoints -= price;
                return true;
            }

            return false;
        }
        
        private void UpdateStaminaUI()
        {
            if (staminaSlider != null) staminaSlider.value = GetStaminaFraction();
            if (staminaText != null) staminaText.text = $"{Mathf.Round(_staminaPoints)}";
            //Debug.Log($"{gameObject.name}: Stamina updated: {_staminaPoints}");
        }
    }
}
