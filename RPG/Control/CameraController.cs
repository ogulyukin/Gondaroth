using System;
using Cinemachine;
using UnityEngine;

namespace RPG.Control
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] GameObject followCamera;
        [SerializeField] GameObject freeLookCamera;
        private CinemachineFreeLook _freeLookComponent;

        private void Awake()
        {
            _freeLookComponent = freeLookCamera.GetComponent<CinemachineFreeLook>();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(1))
            {
                _freeLookComponent.m_XAxis.m_MaxSpeed = 200;
            }
            if (Input.GetMouseButtonUp(1))
            {
                _freeLookComponent.m_XAxis.m_MaxSpeed = 0;
            }
            
            if (Input.mouseScrollDelta.y != 0)
            {
                _freeLookComponent.m_YAxis.m_MaxSpeed = 10;
            }

        }
    }
}
