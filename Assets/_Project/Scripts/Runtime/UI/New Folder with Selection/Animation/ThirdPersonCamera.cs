using UnityEngine;

namespace EnhancedShift.Player.Camera
{
    public class PlayerCamera : MonoBehaviour
    {
        [Header("Sensitivity Settings")]
        [SerializeField] private float sensitivityX = 200f;
        [SerializeField] private float sensitivityY = 200f;

        [Header("Clamping")]
        [SerializeField] private float minXAngle = -80f;
        [SerializeField] private float maxXAngle = 80f;

        [Header("References")]
        [SerializeField] private Transform playerBody;

        private float xRotation = 0f;
        private float yRotation = 0f;
        private bool isCursorLocked = true;

        private void Start()
        {
            SetCursorState(true);
        }

        private void Update()
        {
            HandleCursorToggle();

            if (!isCursorLocked) return;

            Look();
        }

        private void Look()
        {
            float mouseX = Input.GetAxisRaw("Mouse X") * sensitivityX * Time.deltaTime;
            float mouseY = Input.GetAxisRaw("Mouse Y") * sensitivityY * Time.deltaTime;

            yRotation += mouseX;
            xRotation -= mouseY;

            xRotation = Mathf.Clamp(xRotation, minXAngle, maxXAngle);

            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

            if (playerBody != null)
            {
                playerBody.rotation = Quaternion.Euler(0f, yRotation, 0f);
            }
        }

        private void HandleCursorToggle()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SetCursorState(false);
            }

            if (!isCursorLocked && Input.GetMouseButtonDown(0))
            {
                SetCursorState(true);
            }
        }

        private void SetCursorState(bool locked)
        {
            isCursorLocked = locked;
            Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !locked;
        }
    }
}