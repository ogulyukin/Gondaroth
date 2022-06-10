using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace RPG.SceneManagement
{
    public class Portal : MonoBehaviour
    {
        [SerializeField] private int sceneIndex = -1;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private int portalId;
        [SerializeField] private int destination;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (sceneIndex < 0) return;
                if (SceneManager.GetActiveScene().buildIndex != sceneIndex)
                {
                    StartCoroutine(Transition());
                }
                else
                {
                    UpdatePlayer(GetOtherPortal());
                }
                
            }
        }

        public int GetPortalID()
        {
            return portalId;
        }

        private IEnumerator Transition()
        {
            DontDestroyOnLoad(gameObject);
            GetComponent<Collider>().enabled = false;
            var fader = FindObjectOfType<Fader>();
            yield return fader.FadeOut();

            var wrapper = FindObjectOfType<SavingWrapper>();
            wrapper.Save();
            
            yield return SceneManager.LoadSceneAsync(sceneIndex);
            
            wrapper.Load();
            
            var otherPortal = GetOtherPortal();
            UpdatePlayer(otherPortal);
            
            wrapper.Save();
            
            yield return fader.FadeIn();
            Destroy(gameObject);
        }

        private void UpdatePlayer(Portal otherPortal)
        {
            var player = GameObject.FindWithTag("Player");
            player.GetComponent<NavMeshAgent>().Warp(otherPortal.spawnPoint.position);
            player.transform.rotation = otherPortal.spawnPoint.rotation;
        }

        private Portal GetOtherPortal()
        {
            foreach (var portal in FindObjectsOfType<Portal>())
            {
                if(portal == this || portal.GetPortalID() != destination) continue;
                return portal;
            }
            return null;
        }
    }
}
