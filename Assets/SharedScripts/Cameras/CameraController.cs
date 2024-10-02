using System;
using Cinemachine;
using TMPro;
using UnityEngine;

namespace GameMath.Cameras
{
    public class CameraController : MonoBehaviour
    {
        [Header("Cameras")]
        [SerializeField] private IsometricCamera isometricCamera;
        [SerializeField] private CinemachineVirtualCamera topdownCamera;
        [SerializeField] private CinemachineVirtualCamera sidewaysCamera;
        
        [Header("UI")]
        [SerializeField] private TMP_Dropdown cameraViewDropdown;
        
        private void Awake()
        {
            cameraViewDropdown.onValueChanged.AddListener(OnViewChanged);
        }
        
        private void Update()
        {
            HandleCameraInput();
        }

        private void OnViewChanged(int cameraIndex)
        {
            switch (cameraIndex)
            {
                case 0:
                    isometricCamera.Priority = 11;
                    topdownCamera.Priority   = 10;
                    sidewaysCamera.Priority  = 10;
                    break;
            
                case 1:
                    isometricCamera.Priority = 10;
                    topdownCamera.Priority   = 11;
                    sidewaysCamera.Priority  = 10;
                    break;
            
                case 2:
                    isometricCamera.Priority = 10;
                    topdownCamera.Priority   = 10;
                    sidewaysCamera.Priority  = 11;
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private void HandleCameraInput()
        {
            if (Input.GetKeyDown(KeyCode.Q))
                isometricCamera.RotateClockwise();
            else if (Input.GetKeyDown(KeyCode.E))
                isometricCamera.RotateCounterClockwise();
            
            var mouseZoom = Input.mouseScrollDelta;
            isometricCamera.Zoom(mouseZoom.y);
        }
    }
}