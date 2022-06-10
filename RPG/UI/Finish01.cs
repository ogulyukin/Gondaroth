using RPG.Control;
using UnityEngine;

namespace RPG.UI
{
    public class Finish01 : MonoBehaviour
    {
        [SerializeField] private GameObject gameOverCanvas;

        public void OnTriggerEnter(Collider other)
        {
            if(other.GetComponent<PlayerController>() != null) EndGame();
        }

        private void EndGame()
        {
            gameOverCanvas.GetComponent<GameOver>().GameOverHandler(1);
        }
    }
}
