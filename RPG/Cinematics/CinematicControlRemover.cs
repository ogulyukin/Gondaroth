using System;
using RPG.Control;
using RPG.Core;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics
{
    public class CinematicControlRemover : MonoBehaviour
    {
        private PlayableDirector _playableDirector;
        private GameObject _player;

        private void OnEnable()
        {
            _playableDirector.played += DisableControl;
            _playableDirector.stopped += EnableControl;
        }

        private void OnDisable()
        {
            _playableDirector.played -= DisableControl;
            _playableDirector.stopped -= EnableControl;
        }

        private void Awake()
        {
            _playableDirector = GetComponent<PlayableDirector>();
            _player = GameObject.FindWithTag("Player");
        }

        private void DisableControl(PlayableDirector director)
        {
            if (_playableDirector == director)
            {
                _player.GetComponent<ActionScheduler>().CancelCurrentAction();
                _player.GetComponent<PlayerController>().enabled = false;
            }
        }

        private void EnableControl(PlayableDirector director)
        {
            if(_playableDirector == director) _player.GetComponent<PlayerController>().enabled = true;
        }
    }
}
