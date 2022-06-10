using UnityEngine;

namespace RPG.Core
{
    public class DestroyAfterEffect : MonoBehaviour
    {
        [SerializeField] private GameObject targetToDestroy;
        private void Update()
        {
            if (!GetComponent<ParticleSystem>())
            {
                Destroy(targetToDestroy != null ? targetToDestroy : gameObject);
            }
        }
    }
}
