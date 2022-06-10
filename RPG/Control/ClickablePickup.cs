using System;
using RPG.Inventories;
using RPG.OtherPlayerAction;
using UnityEngine;

namespace RPG.Control
{
    [RequireComponent(typeof(Pickup))]
    public class ClickablePickup : MonoBehaviour, IRaycastable
    {
        private Pickup _pickup;

        private void Awake()
        {
            _pickup = GetComponent<Pickup>();
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if (Input.GetMouseButtonDown(0))
            {
                GameObject.FindWithTag("Player").GetComponent<PickupAction>().StartPickup(_pickup);
            }
            
            return true;
        }

        public CursorType GetCursorType()
        {
            if (_pickup.CanBePickedUp())
            {
                return CursorType.PickUp;
            }
            else
            {
                return CursorType.FullPickup;
            }
        }
    }
}
