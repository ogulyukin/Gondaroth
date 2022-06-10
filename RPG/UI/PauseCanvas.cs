using Saving;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.UI
{
    public class PauseCanvas : MonoBehaviour
    {
        [SerializeField] private GameObject timeLine;
        //[SerializeField] private GameObject pauseCanvas;
        [SerializeField] private TMP_Text pauseText;
        //[SerializeField] private GameObject avatarCamera;
        [SerializeField] private GameObject mainGameCanvas;

        private PlayableDirector m_Director;
        private const string DefaultSaveFile = "save";

        private void Start()
        {
            if (timeLine != null)
            {
                m_Director = timeLine.GetComponent<PlayableDirector>();
                m_Director.stopped += ShowHelp;
            }
        }

        public void PauseCanvasHandler()
        {
            gameObject.SetActive(true);
            TurnOffAvatarCamera();
            Time.timeScale = 0f;
        }

        public void ContinueButtonHandler()
        {
            TurnOnAvatarCamera();
            gameObject.SetActive(false);
            Time.timeScale = 1f;
        }

        public void SetPauseText(string pauseTextToShow)
        {
            pauseText.SetText(pauseTextToShow);
        }

        public void TurnOffAvatarCamera()
        {
            //avatarCamera.SetActive(false);
            mainGameCanvas.SetActive(false);
            GetComponent<SavingSystem>().enabled = false;
        }

        public void TurnOnAvatarCamera()
        {
            //avatarCamera.SetActive(true);
            mainGameCanvas.SetActive(true);
            GetComponent<SavingSystem>().enabled = true;
        }

        public void ExitGame()
        {
            Application.Quit();
        }

        public void Save()
        {
            GetComponent<SavingSystem>().Save(DefaultSaveFile);
        }

        public void Load()
        {
            GetComponent<SavingSystem>().Load(DefaultSaveFile);
        }

        private void ShowHelp(PlayableDirector director)
        {
            if (m_Director == director)
            {
                SetPauseText("CONTROLS:\n\nto move and attack use mouse\nuse SHIFT to run\nuse 0 for 1st spell\nuse 9 for 2nd spell\nuse P to pause game\nuse T to taunt enemy");
                PauseCanvasHandler();    
            }
        }
    }
}
