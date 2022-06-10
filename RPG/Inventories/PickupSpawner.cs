using System.Collections.Generic;
using Saving;
using UnityEngine;

namespace RPG.Inventories
{
    /// <summary>
    /// Spawns pickups that should exist on first load in a level. This
    /// automatically spawns the correct prefab for a given inventory item.
    /// </summary>
    ///
    [System.Serializable]
    public class PickUpEntry
    {
        public InventoryItem item;
        public int number;
    }
    public class PickupSpawner : MonoBehaviour, ISaveable
    {
        // CONFIG DATA
        [SerializeField] PickUpEntry[]  items;// = null;

        // LIFECYCLE METHODS
        private void Awake()
        {
            // Spawn in Awake so can be destroyed by save system after.
            SpawnPickup();
        }

        // PUBLIC

        /// <summary>
        /// Returns the pickup spawned by this class if it exists.
        /// </summary>
        /// <returns>Returns null if the pickup has been collected.</returns>
        public Pickup GetPickup() 
        { 
            return GetComponentInChildren<Pickup>();
        }

        /// <summary>
        /// True if the pickup was collected.
        /// </summary>
        public bool isCollected() 
        { 
            return GetPickup() == null;
        }

        //PRIVATE

        private void SpawnPickup()
        {
            if(items.Length == 0) return;
            var spawnedPickup = items[0].item.SpawnPickup(transform.position, items[0].number);
            spawnedPickup.transform.SetParent(transform);
            for (int i = 1; i < items.Length; i++)
            {
                spawnedPickup.AddItemToPickUp(items[i].item, items[i].number);
            }
        }

        private void DestroyPickup()
        {
            if (GetPickup())
            {
                Destroy(GetPickup().gameObject);
            }
        }

        object ISaveable.CaptureState()
        {
            return isCollected();
        }

        void ISaveable.RestoreState(object state)
        {
            bool shouldBeCollected = (bool)state;

            if (shouldBeCollected && !isCollected())
            {
                DestroyPickup();
            }

            if (!shouldBeCollected && isCollected())
            {
                SpawnPickup();
            }
        }
    }
}