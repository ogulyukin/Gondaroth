using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics
{
    public class CinematicTrigger001 : MonoBehaviour
    {
        private bool m_IsActivated;

        private void OnTriggerEnter(Collider other)
        {
            if (m_IsActivated || !other.gameObject.CompareTag("Player")) return;
            GetComponent<PlayableDirector>().Play();
            m_IsActivated = true;
        }
    }
}
