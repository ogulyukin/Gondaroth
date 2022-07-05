using System;
using Cinemachine;
using UnityEngine;

namespace RPG.Control
{
    public class CameraController : MonoBehaviour
    {
        //[SerializeField] GameObject followCamera;
        //[SerializeField] GameObject freeLookCamera;
        //private CinemachineFreeLook _freeLookComponent;
        [SerializeField] private float cameraRotationSpeed = 5f;

        [SerializeField] private float cameraDistanceChangeScale = 0.1f;
        //[SerializeField] CinemachineVirtualCamera followCamera;
        private Cinemachine3rdPersonFollow _cinemachineComponentBase;
        private void Awake()
        {
            //_freeLookComponent = freeLookCamera.GetComponent<CinemachineFreeLook>();
            _cinemachineComponentBase = (Cinemachine3rdPersonFollow)GameObject.FindWithTag("VCam").GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent(CinemachineCore.Stage.Body);
        }

        private void Update()
        {
            if (Input.GetMouseButton(1))
            {
                transform.Rotate(Vector3.up, cameraRotationSpeed * Time.deltaTime * Input.GetAxis("Mouse X"));
                transform.Rotate(Vector3.left, cameraRotationSpeed * Time.deltaTime * Input.GetAxis("Mouse Y"));
            }
            
            if((_cinemachineComponentBase.CameraDistance > 3 && Input.mouseScrollDelta.y < 0) || (_cinemachineComponentBase.CameraDistance < 10 && Input.mouseScrollDelta.y > 0))
            {
                _cinemachineComponentBase.CameraDistance += Input.mouseScrollDelta.y * cameraDistanceChangeScale;
            }
            /*
            if (Input.GetMouseButtonUp(1))
            {
                //_freeLookComponent.m_XAxis.m_MaxSpeed = 0;
            }
            
            if (Input.mouseScrollDelta.y != 0)
            {
                //_freeLookComponent.m_YAxis.m_MaxSpeed = 10;
            }*/

        }
    }
}
