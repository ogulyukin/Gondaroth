using System;
using RPG.Combat;
using RPG.Stats;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Attributes
{
    public class HealthDisplay : MonoBehaviour
    {
        [SerializeField] private Text playerHealth;
        [SerializeField] private Text enemyHealth;
        [SerializeField] private Text playerExp;
        [SerializeField] private Text playerLevel;
        private Health _health;
        private Fighter _fighter;

        private void Awake()
        {
            _health = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
            _fighter = _health.GetComponent<Fighter>();
        }

        private void LateUpdate()
        {
            enemyHealth.text = $"Enemy: {_fighter.GetEnemyHealth()}";
            playerHealth.text = $"Health: {_health.GetHealthDetail()}";
        }
    }
}
