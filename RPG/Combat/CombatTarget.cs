using System;
using RPG.Attributes;
using RPG.Control;
using RPG.Core;
using TMPro;
using UnityEngine;

namespace RPG.Combat
{
    [RequireComponent(typeof(Health))]
    public class CombatTarget : MonoBehaviour, IRaycastable
    {
        [SerializeField] private string unitName = "Unit";
        [SerializeField] private UnitTypes unitType;
        [SerializeField] private TMP_Text nameText;

        private void Start()
        {
            nameText.SetText(unitName);
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if (!callingController.GetComponent<Fighter>().CanAttack(this))
            {
                return false;
            }

            if (Input.GetMouseButton(0))
            {
                callingController.GetComponent<Fighter>().Attack(this);
            }

            return true;
        }

        public CursorType GetCursorType()
        {
            return CursorType.Combat;
        }
        public UnitTypes GetUnitType()
        {
            return unitType;
        }
        
        public bool CheckAttackTarget(UnitTypes type)
        {
            //print($"CheckTarget attacker{unitType} target{type}");
            if (type == unitType) return false;
            if (type == UnitTypes.Elves && (unitType == UnitTypes.DarkElves || unitType == UnitTypes.Monster || unitType == UnitTypes.Animal)) return true;
            if (type == UnitTypes.Monster) return true;
            if (type == UnitTypes.Animal) return true;
            if (type == UnitTypes.DarkElves && (unitType == UnitTypes.Elves || unitType == UnitTypes.Monster || unitType == UnitTypes.Animal)) return true;
            return false;
        }
        
    }
}
