using System.Collections.Generic;
using UnityEngine;

namespace RPG.Inventories
{
    [CreateAssetMenu(menuName = ("RPG/Inventory/Drop Library"))]
    public class DropLibrary : ScriptableObject
    {
        [SerializeField] private float[] dropChancePercentage;
        [SerializeField] private int[] minDrops;
        [SerializeField] private int[] maxDrops;
        [SerializeField] private DropConfig[] potentialDrops;

        [System.Serializable]
        class DropConfig
        {
            public InventoryItem item;
            public float[] relativeChance;
            public int[] minNumber;
            public int[] maxNumber;

            public int GetRandomNumber(int level)
            {
                if (!item.IsStackable()) return 1;
                return Random.Range(GetByLevel(minNumber, level), GetByLevel(maxNumber, level) + 1);
            }
        }

        public struct Dropped
        {
            public InventoryItem item;
            public int number;
        }

        public IEnumerable<Dropped> GetRandomDrops(int level)
        {
            if (!ShouldRandomDrop(level))
            {
                yield break;
            }

            for (var i = 0; i < GetRandomNumberOfDrops(level); i++)
            {
                GetRandomDrop(level);
            }
        }

        private bool ShouldRandomDrop(int level)
        {
            return Random.Range(0, 100) < GetByLevel(dropChancePercentage, level);
        }

        private int GetRandomNumberOfDrops(int level)
        {
            return Random.Range(GetByLevel(minDrops, level), GetByLevel(maxDrops, level));
        }

        private Dropped GetRandomDrop(int level)
        {
            var drop = SelectRandomDropItem(level);
            var result = new Dropped()
            {
                item = drop.item,
                number = drop.GetRandomNumber(level)
            };
            return result;
        }

        DropConfig SelectRandomDropItem(int level)
        {
            var totalChance = GetTotalChance(level);
            var randomRoll = Random.Range(0, totalChance);
            float chanceTotal = 0;
            foreach (var drop in potentialDrops)
            {
                chanceTotal += GetByLevel(drop.relativeChance, level);
                if (totalChance > randomRoll)
                {
                    return drop;
                }
            }

            return null;
        }

        private float GetTotalChance(int level)
        {
            float total = 0;
            foreach (var drop in potentialDrops)
            {
                total += GetByLevel(drop.relativeChance, level);
            }

            return total;
        }

        static T GetByLevel<T>(T[] values, int level)
        {
            if (values.Length == 0)
            {
                return default;
            }

            if (level > values.Length)
            {
                return values[values.Length - 1];
            }

            if (level <= 0)
            {
                return default;
            }

            return values[level - 1];
        }
    }
}
