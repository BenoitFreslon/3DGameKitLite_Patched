using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

namespace Gamekit3D
{
    public class CameraSettings : MonoBehaviour
    {
        public enum InputChoice
        {
            KeyboardAndMouse, Controller,
        }

        [Serializable]
        public struct InvertSettings
        {
            public bool invertX;
            public bool invertY;
        }


        public Transform follow;
        public Transform lookAt;
        public CinemachineFreeLook keyboardAndMouseCamera;
        public CinemachineFreeLook controllerCamera;
        public InputChoice inputChoice;
        public InvertSettings keyboardAndMouseInvertSettings;
        public InvertSettings controllerInvertSettings;
        public bool allowRuntimeCameraSettingsChanges;

        public CinemachineFreeLook Current
        {
            get { return inputChoice == InputChoice.KeyboardAndMouse ? keyboardAndMouseCamera : controllerCamera; }
        }

        void Reset()
        {
            Transform keyboardAndMouseCameraTransform = transform.Find("KeyboardAndMouseFreeLookRig");
            if (keyboardAndMouseCameraTransform != null)
                keyboardAndMouseCamera = keyboardAndMouseCameraTransform.GetComponent<CinemachineFreeLook>();

            Transform controllerCameraTransform = transform.Find("ControllerFreeLookRig");
            if (controllerCameraTransform != null)
                controllerCamera = controllerCameraTransform.GetComponent<CinemachineFreeLook>();

            PlayerController playerController = FindObjectOfType<PlayerController>();
            if (playerController != null && playerController.name == "Ellen")
            {
                follow = playerController.transform;

                lookAt = follow.Find("HeadTarget");

                if (playerController.cameraSettings == null)
                    playerController.cameraSettings = this;
            }
        }

        void Awake()
        {
            UpdateCameraSettings();
            SetupCinemachineInputOverride();
        }

        void Update()
        {
            if (allowRuntimeCameraSettingsChanges)
            {
                UpdateCameraSettings();
            }
            
            // Appliquer l'input avec sensibilité à Cinemachine
            ApplySensitivityToCinemachine();
        }

        void SetupCinemachineInputOverride()
        {
            // Désactiver l'input automatique de Cinemachine pour le contrôler manuellement
            if (keyboardAndMouseCamera != null)
            {
                keyboardAndMouseCamera.m_XAxis.m_InputAxisName = "";
                keyboardAndMouseCamera.m_YAxis.m_InputAxisName = "";
            }
            
            if (controllerCamera != null)
            {
                controllerCamera.m_XAxis.m_InputAxisName = "";
                controllerCamera.m_YAxis.m_InputAxisName = "";
            }
        }

        void ApplySensitivityToCinemachine()
        {
            if (PlayerInput.Instance == null)
                return;

            Vector2 cameraInput = PlayerInput.Instance.CameraInput;
            
            CinemachineFreeLook currentCam = Current;
            if (currentCam != null)
            {
                // Appliquer l'input avec la sensibilité déjà intégrée
                currentCam.m_XAxis.m_InputAxisValue = cameraInput.x;
                currentCam.m_YAxis.m_InputAxisValue = cameraInput.y;
            }
        }

        void UpdateCameraSettings()
        {
            keyboardAndMouseCamera.Follow = follow;
            keyboardAndMouseCamera.LookAt = lookAt;
            keyboardAndMouseCamera.m_XAxis.m_InvertInput = keyboardAndMouseInvertSettings.invertX;
            keyboardAndMouseCamera.m_YAxis.m_InvertInput = keyboardAndMouseInvertSettings.invertY;

            controllerCamera.m_XAxis.m_InvertInput = controllerInvertSettings.invertX;
            controllerCamera.m_YAxis.m_InvertInput = controllerInvertSettings.invertY;
            controllerCamera.Follow = follow;
            controllerCamera.LookAt = lookAt;

            keyboardAndMouseCamera.Priority = inputChoice == InputChoice.KeyboardAndMouse ? 1 : 0;
            controllerCamera.Priority = inputChoice == InputChoice.Controller ? 1 : 0;
        }
    } 
}
