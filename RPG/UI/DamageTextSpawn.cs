using System;
using TMPro;
using UnityEngine;

namespace RPG.UI
{
    public class DamageTextSpawn : MonoBehaviour
    {
        [SerializeField] private GameObject damageText;

        public void Spawn(float damage)
        {
            var instance = Instantiate(damageText, transform);
            instance.GetComponentInChildren<TextMeshProUGUI>().text = $"{Mathf.RoundToInt(damage)}";
            instance.GetComponent<Animator>().SetTrigger("ShowDamage");
            Debug.Log($"{gameObject.name} Instantiate damage text {damage}");
        }

        public void DisableCanvas()
        {
            Debug.Log($"{gameObject.name} Disabling name canvas");
            gameObject.SetActive(false);
        }
    }
}
