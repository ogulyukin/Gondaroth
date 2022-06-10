using System.Collections.Generic;
using Saving;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG.Inventories
{
    public class ItemDropper : MonoBehaviour, ISaveable
    {
        protected Pickup _droppedItems;
        private List<DropRecord> _otherSceneDroppedItems = new List<DropRecord>();
        
        public void DropItem(InventoryItem item, int number)
        {
            FillPickup(item, GetDropLocation(), number);
        }

        protected virtual Vector3 GetDropLocation()
        {
            return transform.position;
        }

        private void FillPickup(InventoryItem item, Vector3 spawnLocation, int number)
        {
            if (_droppedItems == null)
            {
                _droppedItems = item.SpawnPickup(spawnLocation, number);
                return;
            }
            _droppedItems.AddItemToPickUp(item, number);
        }

        [System.Serializable]
        private struct DropRecord
        {
            public string itemID;
            public SerializableVector3 position;
            public int number;
            public int sceneBuiltIndex;
        }

        object ISaveable.CaptureState()
        {
            RemoveDestroyedDrops();
            var droppedItemsList = new List<DropRecord>();
            var buildIndex = SceneManager.GetActiveScene().buildIndex;
            var position = new SerializableVector3(_droppedItems.transform.position);
            foreach(var item  in _droppedItems.GetItems())
            {
                var droppedItem = new DropRecord
                {
                    itemID = item.GetItemID(),
                    position = position,
                    number = _droppedItems.GetNumber(item),
                    sceneBuiltIndex = buildIndex
                };
                droppedItemsList.Add(droppedItem);
            }
            droppedItemsList.AddRange(_otherSceneDroppedItems);
            return droppedItemsList;
        }

        void ISaveable.RestoreState(object state)
        {
            var droppedItemsList = (List<DropRecord>)state;
            var buildIndex = SceneManager.GetActiveScene().buildIndex;
            _otherSceneDroppedItems.Clear();
            foreach (var item in droppedItemsList)
            {
                if (item.sceneBuiltIndex != buildIndex)
                {
                    _otherSceneDroppedItems.Add(item);
                    continue;
                } 
                var pickupItem = InventoryItem.GetFromID(item.itemID);
                var position = item.position.ToVector();
                var number = item.number;
                FillPickup(pickupItem, position, number);
            }
        }

        private void RemoveDestroyedDrops()
        {/*
            var newList = new List<Pickup>();
            foreach (var item in _droppedItems)
            {
                if (item.V != null)
                {
                    newList.Add(item);
                }
            }
            _droppedItems = newList;*/
        }
    }
}