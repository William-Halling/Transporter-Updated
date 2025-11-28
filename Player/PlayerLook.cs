using UnityEngine;

using Transporter.Inputs;

namespace Transporter.Gameplay
{
    public class PlayerLook : MonoBehaviour
    {
        [Header("Look")]
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private float horizontalSensitivity = 2f;
        [SerializeField] private float verticalSensitivity = 2f;
        [SerializeField] private float maxLookAngle = 89f;


        private GameInput _playerInput;
        private float _xRotation = 0f;

        private void Start()
        {
            _playerInput     = GameInput.Instance;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible   = false;


            if (cameraTransform == null && Camera.main != null)
            {
                cameraTransform = Camera.main.transform;
            }
        }


        private void Update()
        {
            if (_playerInput == null)
            { 
                return;
            }


            HandleLook();
        }



        private void HandleLook()
        {
            float mouseX = _playerInput.LookInput.x * horizontalSensitivity;
            float mouseY = _playerInput.LookInput.y * verticalSensitivity;

            _xRotation -= mouseY;
            _xRotation = Mathf.Clamp(_xRotation, -maxLookAngle, maxLookAngle);

            
            if (cameraTransform != null)
            { 
                cameraTransform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
            }

            transform.Rotate(Vector3.up * mouseX);
        }
    }
}
