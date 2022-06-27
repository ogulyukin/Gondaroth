using RPG.Stats;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Attributes
{
    public class Mana : MonoBehaviour
    {
        private float _manaPoints;
        [SerializeField] private Slider manaSlider;
        [SerializeField] private TMP_Text manaText;
        [SerializeField] private float manaRegenerationRate = 0.01f;
        private BaseStats _baseStats;

        private void Awake()
        {
            _baseStats = GetComponent<BaseStats>();
        }

        private void Start()
        {
            _manaPoints = GetComponent<BaseStats>().GetStat(MainStats.Intellect);
            UpdateManaUI();
        }
        private void Update()
        {
            RestoreMana(manaRegenerationRate);
        }

        public float GetCurrentManaLevel()
        {
            return _manaPoints;
        }
        public bool CheckManaAvaible(float mana)
        {
            return mana < _manaPoints;
        }

        public void SpendMana(float manaAmount)
        {
            _manaPoints = Mathf.Max(_manaPoints - manaAmount, 0);
            UpdateManaUI();
        }

        public void RestoreMana(float manaAmount)
        {
            var totalMana = _baseStats.GetStat(MainStats.Intellect);
            _manaPoints += manaAmount;
            if (_manaPoints > totalMana) _manaPoints = totalMana;
            UpdateManaUI();
        }

        private float GetManaFraction()
        {
            return _manaPoints/_baseStats.GetStat(MainStats.Intellect);
        }
        private void UpdateManaUI()
        {
            if (manaSlider != null) manaSlider.value = GetManaFraction();
            if (manaText != null) manaText.text = $"{Mathf.Round(_manaPoints)}";
        }
    }
}
