using UnityEngine;
using UnityEngine.Serialization;

namespace RPG.Inventories
{
    /// <summary>
    /// An inventory item that can be equipped to the player. Weapons could be a
    /// subclass of this.
    /// </summary>
    [CreateAssetMenu(menuName = ("RPG/Inventory/Equipable Item"))]
    public class EquipableItem : InventoryItem
    {
        // CONFIG DATA
        [Tooltip("Where are we allowed to put this item.")]
        [SerializeField] EquipLocation allowedEquipLocation = EquipLocation.Weapon;
        [SerializeField] private int[] itemPrefabIndexes;
        [SerializeField] private GameObject itemPrefab;
        [SerializeField] private bool externalItem = false;
        [SerializeField] private float strengthRequired;

        // PUBLIC

        public EquipLocation GetAllowedEquipLocation()
        {
            return allowedEquipLocation;
        }

        public int[] GetItemPrefabIndexes()
        {
            return itemPrefabIndexes;
        }

        public GameObject GetItemPrefab()
        {
            return itemPrefab;
        }

        public bool IsExternal()
        {
            return externalItem;
        }

        public float GetStrengthRequired()
        {
            return strengthRequired;
        }
    }
}