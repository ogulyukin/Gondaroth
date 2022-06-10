using RPG.SceneManagement;
using UnityEngine;

namespace RPG.Core
{
    public class PersistentObjectSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject persistentObjectPrefab;
        [SerializeField] private float fadingOutTime = 2f;
        [SerializeField] private float fadingInTime = 1f;
        [SerializeField] private float fadeWaitTime = 0.5f;
        private static bool _hasSpawned;

        private void Awake()
        {
            if (_hasSpawned) return;
            SpawnPersistentObjects();
            _hasSpawned = true;
        }

        private void SpawnPersistentObjects()
        {
            var persistentObject = Instantiate(persistentObjectPrefab);
            FindObjectOfType<Fader>().SetTimeOuts(new Vector3(fadingOutTime, fadingInTime, fadeWaitTime));
            DontDestroyOnLoad(persistentObject);
        }
    }
}
