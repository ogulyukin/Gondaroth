using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPG.Inventories
{
    /// <summary>
    /// To be placed at the root of a Pickup prefab. Contains the data about the
    /// pickup such as the type of item and the number.
    /// </summary>
    public class Pickup : MonoBehaviour
    {
        // STATE
        Dictionary<InventoryItem, int> items = new Dictionary<InventoryItem, int>();

        // CACHED REFERENCE
        Inventory inventory;

        // LIFECYCLE METHODS

        protected void Awake()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            inventory = player.GetComponent<Inventory>();
        }

        public bool IsPickupEmpty()
        {
            return items.Count == 0;
        }
        
        public void AddItemToPickUp(InventoryItem item, int number)
        {
            if (items.ContainsKey(item))
            {
                if (item.IsStackable()) items[item] += number;
            }
            else
            {
                items.Add(item, number);
            }
        }
        
        // PUBLIC

        /// <summary>
        /// Set the vital data after creating the prefab.
        /// </summary>
        /// <param name="item">The type of item this prefab represents.</param>
        /// <param name="number">The number of items represented.</param>
        public void Setup(Dictionary<InventoryItem, int> items)
        {
            foreach (var item in items)
            {
                items.Add(item.Key, !item.Key.IsStackable() ? 1 : item.Value);    
            }
        }

        public IEnumerable<InventoryItem> GetItems()
        {
            foreach (var item in items)
            {
                yield return item.Key;
            }
        }

        public int GetNumber(InventoryItem item)
        {
            return items.ContainsKey(item) ? items[item] : 0;
        }

        public void PickupItem()
        {

            var keys = items.Keys;
            foreach (var key in keys.ToList())
            {
                bool foundSlot = inventory.AddToFirstEmptySlot(key, items[key]);
                if(!foundSlot) return;
                items.Remove(key);
            }
            /*
            foreach (var item in items)
            {
                bool foundSlot = inventory.AddToFirstEmptySlot(item.Key, item.Value);    
                if(!foundSlot) return;
                items.Remove(item.Key);
            }*/
            Destroy(gameObject);
        }

        public bool CanBePickedUp()
        {
            foreach (var item in items)
            {
                if (inventory.HasSpaceFor(item.Key))
                {
                    return true;
                }    
            }

            return false;
        }
    }
}