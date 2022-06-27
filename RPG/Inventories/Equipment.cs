using System;
using System.Collections.Generic;
using RPG.Combat;
using RPG.Stats;
using Saving;
using UnityEngine;

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
        Dictionary<EquipLocation, EquipableItem> _equippedItems = new Dictionary<EquipLocation, EquipableItem>();

        private void Start()
        {
            Debug.Log($"Start begins {gameObject.name}");
            if(GetItemInSlot(EquipLocation.Body) == null) ToggleDefault(true, EquipLocation.Body);
            if(GetItemInSlot(EquipLocation.Trousers) == null) ToggleDefault(true, EquipLocation.Trousers);
            if(GetItemInSlot(EquipLocation.Helmet) == null) ToggleDefault(true, EquipLocation.Helmet);
            EquipmentUpdated?.Invoke();
            Debug.Log($"Start ends {gameObject.name}");
        }
        // PUBLIC
        
        /// <summary>
        /// Broadcasts when the items in the slots are added/removed.
        /// </summary>
        public event Action EquipmentUpdated;

        /// <summary>
        /// Return the item in the given equip location.
        /// </summary>
        public EquipableItem GetItemInSlot(EquipLocation equipLocation)
        {
            if (!_equippedItems.ContainsKey(equipLocation))
            {
                return null;
            }

            return _equippedItems[equipLocation];
        }

        public bool IsItemEquipped(EquipLocation location)
        {
            return _equippedItems.ContainsKey(location);
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
            _equippedItems[slot] = item;
            ToggleDefault(false, slot);
            ShowHideWearables(item.GetItemPrefabIndexes(), true);
            EquipmentUpdated?.Invoke();
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

        private void DestroyItem(EquipLocation slot)
        {
            if (slot == EquipLocation.Helmet)
            {
                var tempItem = GetComponent<ArmorList>().head.Find(_helmetName);
                if (tempItem != null) Destroy(tempItem.gameObject);
            }
            if (slot == EquipLocation.Shield)
            {
                var tempItem = GetComponent<ArmorList>().shield.Find(_shieldName);
                if (tempItem != null) Destroy(tempItem.gameObject);
            }
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
            if (_equippedItems.ContainsKey(slot))
            {
                ShowHideWearables(_equippedItems[slot].GetItemPrefabIndexes(),false);
                var item = _equippedItems[slot];
                _equippedItems.Remove(slot);
                if (item.IsExternal() && item != null)
                {
                    DestroyItem(slot);
                }
            }
            
            ToggleDefault(true, slot);
            EquipmentUpdated?.Invoke();
        }

        /// <summary>
        /// Enumerate through all the slots that currently contain items.
        /// </summary>
        protected IEnumerable<EquipLocation> GetAllPopulatedSlots()
        {
            return _equippedItems.Keys;
        }
        /*
        private void CheckBeforeRemoveDefault()
        {
            if (!_equippedItems.ContainsKey(EquipLocation.Body) &&
                !_equippedItems.ContainsKey(EquipLocation.ClothesUpward)) ToggleDefault(true, EquipLocation.Body);
            if(!_equippedItems.ContainsKey(EquipLocation.Trousers) && !_equippedItems.ContainsKey(EquipLocation.ClothesDownward)) ToggleDefault(true, EquipLocation.Trousers);
        }*/
        // PRIVATE
        private void ToggleDefault(bool flag, EquipLocation location)
        {
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
            foreach (var item in items)
            {
                item.SetActive(flag);
            }
        }

        private bool CanEquipItem(EquipableItem item, EquipLocation location)
        {
            if (item.GetStrengthRequired() > GetComponent<BaseStats>().GetStat(MainStats.Strength)) return false;
            if (location == EquipLocation.Shield)
            {
                if (!_equippedItems.ContainsKey(EquipLocation.Weapon)) return true;
                var weapon = (WeaponConfig)_equippedItems[EquipLocation.Weapon];
                return weapon.GetWeaponHand();
            }else if (location == EquipLocation.Weapon)
            {
                if (!_equippedItems.ContainsKey(EquipLocation.Shield)) return true;
                return ((WeaponConfig)item).GetWeaponHand();
            }
            
            return true;
        }
        object ISaveable.CaptureState()
        {
            var equippedItemsForSerialization = new Dictionary<EquipLocation, string>();
            foreach (var pair in _equippedItems)
            {
                equippedItemsForSerialization[pair.Key] = pair.Value.GetItemID();
            }
            return equippedItemsForSerialization;
        }

        void ISaveable.RestoreState(object state)
        {
            _equippedItems = new Dictionary<EquipLocation, EquipableItem>();

            var equippedItemsForSerialization = (Dictionary<EquipLocation, string>)state;

            foreach (var pair in equippedItemsForSerialization)
            {
                var item = (EquipableItem)InventoryItem.GetFromID(pair.Value);
                if (item != null)
                {
                    _equippedItems[pair.Key] = item;
                }
            }
        }
    }
}