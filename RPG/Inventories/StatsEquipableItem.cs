using System;
using System.Collections.Generic;
using RPG.Stats;
using UnityEngine;

namespace RPG.Inventories
{
    [CreateAssetMenu(menuName = ("RPG/Inventory/Stats Equipable Item"))]
    public class StatsEquipableItem : EquipableItem, IModifierProvider
    {
        [SerializeField] private Modifier[] additiveModifiers;
        [SerializeField] private Modifier[] percentageModifiers;
        
        [System.Serializable]
        struct Modifier
        {
            public MainStats stat;
            public float value;
        }

        public IEnumerable<float> GetAdditiveModifier(MainStats stat)
        {
            foreach (var modifier in additiveModifiers)
            {
                if (modifier.stat == stat) yield return modifier.value;
            }
        }

        public IEnumerable<float> GetPercentageModifier(MainStats stat)
        {
            foreach (var modifier in percentageModifiers)
            {
                if (modifier.stat == stat) yield return modifier.value;
            }
        }
    }
}
