using System;
using RPG.Attributes;
using RPG.Core;
using RPG.Inventories;
using RPG.Movement;
using UnityEngine;

namespace RPG.OtherPlayerAction
{
    public class PickupAction : MonoBehaviour, IAction
    {
        [SerializeField] private float pickupDistance = 1.0f;
        private Pickup _pickup;

        private void Update()
        {
            if(_pickup == null || !GetComponent<Health>().IsAlive()) return;
            if (!GetIsInRange(_pickup.transform))
            {
                GetComponent<Mover>().Moveto(_pickup.transform.position);
            }
            else
            {
                GetComponent<Mover>().Cancel();
                _pickup.PickupItem();
            }
        }

        public void StartPickup(Pickup pickup)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            _pickup = pickup;
        }
        
        private bool GetIsInRange(Transform targetTransform)
        {
            return Vector3.Distance(transform.position, targetTransform.position) < pickupDistance;
        }

        public void Cancel()
        {
            _pickup = null;
            GetComponent<Mover>().Cancel();
        }
    }
}
