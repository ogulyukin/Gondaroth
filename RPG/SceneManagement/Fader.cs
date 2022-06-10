using System.Collections;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class Fader : MonoBehaviour
    {
        private float _fadingOutTime;
        private float _fadingInTime;
        private float _fadeWaitTime;
        private CanvasGroup _canvasGroup;
        private Coroutine _currentActiveFade;
        
        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public void FadeOutImmediate()
        {
            _canvasGroup.alpha = 1;
        }

        public void SetTimeOuts(Vector3 timeOuts)
        {
            _fadingOutTime = timeOuts.x;
            _fadingInTime = timeOuts.y;
            _fadeWaitTime = timeOuts.z;
        }
        public IEnumerator FadeOut()
        {
            PrepareFideInOut();
            _currentActiveFade =  StartCoroutine(FadeRoutine(true));
            yield return _currentActiveFade;
        }

        private void PrepareFideInOut()
        {
            if (_canvasGroup == null) _canvasGroup = GetComponent<CanvasGroup>();
            if (_currentActiveFade != null)
            {
                StopCoroutine(_currentActiveFade);
            }
        }

        public IEnumerator FadeIn()
        {
            if(_canvasGroup == null) _canvasGroup = GetComponent<CanvasGroup>();
            if (_currentActiveFade != null)
            {
                StopCoroutine(_currentActiveFade);
            }
            yield return new WaitForSeconds(_fadeWaitTime);
            _currentActiveFade = StartCoroutine(FadeRoutine(false));
            yield return _currentActiveFade;
        }
        
        private IEnumerator FadeRoutine(bool inOutFade)
        {
            while (!Mathf.Approximately(_canvasGroup.alpha, inOutFade ? 1 : 0))
            {
                _canvasGroup.alpha += (Time.deltaTime / _fadingOutTime) * (inOutFade ? 1 : -1);
                yield return null;
                if (_canvasGroup == null) break;
            }
        }
    }
}
