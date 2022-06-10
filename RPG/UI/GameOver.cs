using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG.UI
{
    public class GameOver : MonoBehaviour
    {
        [SerializeField] private GameObject gameOverCanvas;
        [SerializeField] private TMP_Text gameOverText;
        [SerializeField] private GameObject avatarCamera;
        [SerializeField] private GameObject mainGameCanvas;
        
        public void SetGameOverText(string textToShow)
        {
            gameOverText.SetText(textToShow);
        }
        public void GameOverHandler(int mode)
        {
            if(mode == 0) gameOverText.SetText("GAME OVER\n\nShe was too young. To unskilled for such hurd enemies...");
            if(mode == 1) gameOverText.SetText("This demo is ended here. <i>Elende</i> successfully report to guards about incoming dark elf raiding party.\n" +
                                               "But her trainer <i>Aurion</i> will not happy how she complete her challenge...");
            gameOverCanvas.SetActive(true);
            TurnOffAvatarCamera();
            Time.timeScale = 0f;
        }

        public void RestartButtonHandler()
        {
            TurnOnAvatarCamera();
            gameOverCanvas.SetActive(false);
            Time.timeScale = 1f;
            SceneManager.LoadScene(0);
        }
    
        public void ExitButtonHandler()
        {
            Application.Quit();
        }

        public void TurnOffAvatarCamera()
        {
            avatarCamera.SetActive(false);
            mainGameCanvas.SetActive(false);
        }

        public void TurnOnAvatarCamera()
        {
            avatarCamera.SetActive(true);
            mainGameCanvas.SetActive(true);
        }
    }
}
