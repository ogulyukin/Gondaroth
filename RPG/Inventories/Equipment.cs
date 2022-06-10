using System;
using System.Collections.Generic;
using RPG.Combat;
using RPG.Stats;
using Saving;
using UnityEngine;
using UnityEngine.Serialization;

namespace RPG.Inventories
{
    /// <summary>
    /// Provides a store for the items equipped to a player. Items are stored by
    /// their equip locations.
    /// 
    /// This component should be placed on the GameObject tagged "Player".
    /// </summary>
    public class Equipment : MonoBehaviour, ISaveable
    {
        private string _helmetName = "helm";
        private string _shieldName = "shield"; 
        // STATE
        Dictionary<EquipLocation, EquipableItem> equippedItems = new Dictionary<EquipLocation, EquipableItem>();

        private void Start()
        {
            Debug.Log($"Start begins {gameObject.name}");
            if(GetItemInSlot(EquipLocation.Body) == null) ToggleDefault(true, EquipLocation.Body);
            if(GetItemInSlot(EquipLocation.Trousers) == null) ToggleDefault(true, EquipLocation.Trousers);
            if(GetItemInSlot(EquipLocation.Helmet) == null) ToggleDefault(true, EquipLocation.Helmet);
            equipmentUpdated?.Invoke();
            Debug.Log($"Start ends {gameObject.name}");
        }
        // PUBLIC
        
        /// <summary>
        /// Broadcasts when the items in the slots are added/removed.
        /// </summary>
        public event Action equipmentUpdated;

        /// <summary>
        /// Return the item in the given equip location.
        /// </summary>
        public EquipableItem GetItemInSlot(EquipLocation equipLocation)
        {
            if (!equippedItems.ContainsKey(equipLocation))
            {
                return null;
            }

            return equippedItems[equipLocation];
        }

        public bool IsItemEquipped(EquipLocation location)
        {
            return equippedItems.ContainsKey(location);
        }

        /// <summary>
        /// Add an item to the given equip location. Do not attempt to equip to
        /// an incompatible slot.
        /// </summary>
        public void AddItem(EquipLocation slot, EquipableItem item)
        {
            if (!CanEquipItem(item, slot))
            {
                GetComponent<Inventory>().AddToFirstEmptySlot(item, 1);
                return;
            }
            Debug.Assert(item.GetAllowedEquipLocation() == slot);
            equippedItems[slot] = item;
            ToggleDefault(false, slot);
            ShowHideWearables(item.GetItemPrefabIndexes(), true);
            equipmentUpdated?.Invoke();
            if (item.IsExternal())
            {
                CreateItem(slot, item);
            }
        }

        private void CreateItem(EquipLocation slot, EquipableItem item)
        {
            if (slot == EquipLocation.Helmet)
            {
                var newItem = Instantiate(item.GetItemPrefab(), GetComponent<ArmorList>().head);
                newItem.name = _helmetName;
            }
            if (slot == EquipLocation.Shield)
            {
                var newItem = Instantiate(item.GetItemPrefab(), GetComponent<ArmorList>().shield);
                newItem.name = _shieldName;
            }
        }

        private void DestroyItem(EquipLocation slot, EquipableItem item)
        {
            if (slot == EquipLocation.Helmet)
            {
                DestroyNamedItem(_helmetName);
            }
            if (slot == EquipLocation.Shield)
            {
                DestroyNamedItem(_shieldName);
            }
        }

        private void DestroyNamedItem(string itemName)
        {
            var tempItem = GetComponent<ArmorList>().head.Find(itemName);
            if (tempItem != null) Destroy(tempItem.gameObject);
        }

        private void ShowHideWearables(int[] indexes, bool flag)
        {
            foreach (var i in indexes)
            {
                GetComponent<ArmorList>().armorItem[i].SetActive(flag);
            }
        }

        /// <summary>
        /// Remove the item for the given slot.
        /// </summary>
        public void RemoveItem(EquipLocation slot)
        {
            if (equippedItems.ContainsKey(slot))
            {
                ShowHideWearables(equippedItems[slot].GetItemPrefabIndexes(),false);
                var item = equippedItems[slot];
                equippedItems.Remove(slot);
                if (item.IsExternal() && item != null)
                {
                    DestroyItem(slot, item);
                }
            }
            
            ToggleDefault(true, slot);
            equipmentUpdated?.Invoke();
        }

        /// <summary>
        /// Enumerate through all the slots that currently contain items.
        /// </summary>
        public IEnumerable<EquipLocation> GetAllPopulatedSlots()
        {
            return equippedItems.Keys;
        }

        private void CheckBeforeRemoveDefault()
        {
            if (!equippedItems.ContainsKey(EquipLocation.Body) &&
                !equippedItems.ContainsKey(EquipLocation.ClothesUpward)) ToggleDefault(true, EquipLocation.Body);
            if(!equippedItems.ContainsKey(EquipLocation.Trousers) && !equippedItems.ContainsKey(EquipLocation.ClothesDownward)) ToggleDefault(true, EquipLocation.Trousers);
        }
        // PRIVATE
        private void ToggleDefault(bool flag, EquipLocation location)
        {
            //Debug.Log($"Call ToggleDefault: {flag} : {location}");
            switch (location)
            {
                case EquipLocation.Helmet:
                    ToggleDefaultItems(flag, GetComponent<ArmorList>().defaultItemHead);
                    break;
                case EquipLocation.ClothesUpward:
                    ToggleDefaultItems(flag, GetComponent<ArmorList>().defaultItemBody);
                    break;
                case EquipLocation.Body:
                    ToggleDefaultItems(flag, GetComponent<ArmorList>().defaultItemBody);
                    break;
                case EquipLocation.ClothesDownward:
                    ToggleDefaultItems(flag, GetComponent<ArmorList>().defaultItemTrousers);
                    break;
                case EquipLocation.Trousers:
                    ToggleDefaultItems(flag, GetComponent<ArmorList>().defaultItemTrousers);
                    break;
            }
        }
        

        private static void ToggleDefaultItems(bool flag, GameObject[] items)
        {
            //Debug.Log($"Insade Toggle: {flag} : {items.Length}");
            foreach (var item in items)
            {
                //Debug.Log($"Add item: {flag} : {item.name}");
                item.SetActive(flag);
            }
        }

        private bool CanEquipItem(EquipableItem item, EquipLocation location)
        {
            if (item.GetStrengthRequired() > GetComponent<BaseStats>().GetStat(MainStats.Strength)) return false;
            if (location == EquipLocation.Shield)
            {
                if (!equippedItems.ContainsKey(EquipLocation.Weapon)) return true;
                var weapon = (WeaponConfig)equippedItems[EquipLocation.Weapon];
                return weapon.GetWeaponHand();
            }else if (location == EquipLocation.Weapon)
            {
                if (!equippedItems.ContainsKey(EquipLocation.Shield)) return true;
                return ((WeaponConfig)item).GetWeaponHand();
            }
            
            return true;
        }
        object ISaveable.CaptureState()
        {
            var equippedItemsForSerialization = new Dictionary<EquipLocation, string>();
            foreach (var pair in equippedItems)
            {
                equippedItemsForSerialization[pair.Key] = pair.Value.GetItemID();
            }
            return equippedItemsForSerialization;
        }

        void ISaveable.RestoreState(object state)
        {
            equippedItems = new Dictionary<EquipLocation, EquipableItem>();

            var equippedItemsForSerialization = (Dictionary<EquipLocation, string>)state;

            foreach (var pair in equippedItemsForSerialization)
            {
                var item = (EquipableItem)InventoryItem.GetFromID(pair.Value);
                if (item != null)
                {
                    equippedItems[pair.Key] = item;
                }
            }
        }
    }
}