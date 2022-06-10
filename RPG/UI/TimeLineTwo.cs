using RPG.Control;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.UI
{
    public class TimeLineTwo : MonoBehaviour
    {
        [SerializeField] private GameObject director;
        private bool m_IsActivated = false;

        public void OnTriggerEnter(Collider other)
        {
            if(other.GetComponent<PlayerController>() != null && !m_IsActivated) director.GetComponent<PlayableDirector>().Play();
        }
    }
}
