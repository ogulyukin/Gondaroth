using RPG.Stats;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Inventories
{
    public class RandomDropper : ItemDropper
    {
        [SerializeField] private float scatterDistance = 1;
        [SerializeField] private DropLibrary dropLibrary;
        [SerializeField] private int numberOfDrops = 1;
        private const int Attempts = 20;
        
        public void RandomDrop()
        {
            for (var i = 0; i < numberOfDrops; i++)
            {
                var items = dropLibrary.GetRandomDrops(GetComponent<BaseStats>().GetDropLevel());
                foreach (var item in items)
                {
                    DropItem(item.item, item.number);    
                }
                
            }
        }
        protected override Vector3 GetDropLocation()
        {
            for (var i = 0; i < Attempts; i++)
            {
                var randomPoint = transform.position + Random.insideUnitSphere * scatterDistance;
                if (NavMesh.SamplePosition(randomPoint, out var hit, 0.1f, NavMesh.AllAreas))
                {
                    return hit.position;
                }    
            }

            return transform.position;
        }
    }
}
