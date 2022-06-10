using System.Collections;
using RPG.Attributes;
using RPG.Control;
using UnityEngine;

namespace RPG.Combat
{
    public class WeaponPickup : MonoBehaviour, IRaycastable
    {
        [SerializeField] private WeaponConfig weaponConfig;
        [SerializeField] private float healthToRestore;
        [SerializeField] private float respawnTimeout = 5;
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                PickUp(other.gameObject);
            }
        }

        private void PickUp(GameObject subject)
        {
            if (weaponConfig != null)
            {
                subject.GetComponent<Fighter>().EquipWeapon(weaponConfig);
            }
            if(healthToRestore > 0) subject.GetComponent<Health>().Heal(healthToRestore);
            StartCoroutine(HideForSeconds());
        }

        private IEnumerator HideForSeconds()
        {
            ShowPickup(false);
            yield return new WaitForSeconds(respawnTimeout);
            ShowPickup(true);
        }

        private void ShowPickup(bool mode)
        {
            GetComponent<SphereCollider>().enabled = mode;
            for (var i = 0 ; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(mode);
            }
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if (Input.GetMouseButton(0))
            {
                PickUp(callingController.gameObject);
            }
            return true;
        }

        public CursorType GetCursorType()
        {
            return CursorType.PickUp;
        }
    }
}
