using RPG.Combat;
using RPG.Core;
using RPG.Stats;
using Saving;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] private TakeDamageEvent takeDamage;
        [SerializeField] private UnityEvent die;
        [SerializeField] private Slider healthSlider;
        [SerializeField] private TMP_Text helthText;
        [SerializeField] private GameObject damageText;
        [SerializeField] private GameObject unitCanvas;
        private float _healthPoints = -1f;
        private bool _isAlive = true;
        private  BaseStats _baseStat;
        private static readonly int Death = Animator.StringToHash("Death");
        private static readonly int ShowDamage = Animator.StringToHash("ShowDamage");

        [System.Serializable]
        public class TakeDamageEvent: UnityEvent<float>{}
        
        private void Start()
        {
            if (_healthPoints < 0) _healthPoints = _baseStat.GetStat(MainStats.Strength);
            UpdateHeathUI();
        } 
        private void Awake()
        {
            _baseStat = GetComponent<BaseStats>();
        }
        
        public bool IsAlive()
        {
            return _isAlive;
        }
        
        public float GetHeathFraction()
        {
            return _healthPoints/_baseStat.GetStat(MainStats.Strength);
        }
        public string GetHealthDetail()
        {
            return $"{_healthPoints}/{_baseStat.GetStat(MainStats.Strength)}";
        }
        
        public void TakeDamage(float damage)
        {
            if(!_isAlive || damage < 0) return;
            _healthPoints = Mathf.Max(_healthPoints - damage, 0);
            takeDamage?.Invoke(damage);
            UpdateHeathUI();
            if(damageText != null) SpawnDamageText(damage);
            CheckIfIsAlive();
        }
        
        private void SpawnDamageText(float damage)
        {
            var instance = Instantiate(damageText, transform);
            instance.GetComponentInChildren<TextMeshProUGUI>().text = damage.ToString();
            instance.GetComponent<Animator>().SetTrigger(ShowDamage);
            Debug.Log($"{gameObject.name} Instantiate damage text {damage}");
        }

        private void UpdateHeathUI()
        {
            if (healthSlider != null) healthSlider.value = GetHeathFraction();
            if (helthText != null) helthText.text = $"{_healthPoints}";
            //Debug.Log($"{gameObject.name}: Health updated: {_healthPoints}");
        }

        private void CheckIfIsAlive()
        {
            if (_healthPoints <= 0)
            {
                _isAlive = false;
                Die();
            }
            else
            {
                _isAlive = true;
            }
        }
        public object CaptureState()
        {
            return _healthPoints;
        }

        public void RestoreState(object state)
        {
            _healthPoints = (float)state;
            CheckIfIsAlive();
        }

        public float GetHealthPoints()
        {
            return _healthPoints;
        }

        public float GetMaxHealthPoints()
        {
            return GetComponent<BaseStats>().GetStat(MainStats.Strength);
        }
        
        private void Die()
        {
            GetComponent<ActionScheduler>().CancelCurrentAction();
            GetComponent<Animator>().SetTrigger(Death);
            die?.Invoke();
            if(unitCanvas != null) unitCanvas.SetActive(false);
            GetComponent<Fighter>().enabled = false;
        }

        public void Heal(float healthToRestore)
        {
            _healthPoints = Mathf.Min(healthToRestore + _healthPoints, GetMaxHealthPoints());
            UpdateHeathUI();
        }
    }
}
