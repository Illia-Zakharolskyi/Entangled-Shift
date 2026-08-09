// UnityEngine using directives
using UnityEngine;
using UnityEngine.InputSystem;


    public class PlayerMouseLook : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float sensitivity = 0.1f;
        [SerializeField] private float smoothTime = 0.05f;

        [Header("Vertical Clamp")]
        [SerializeField] private float minY = -80f;
        [SerializeField] private float maxY = 80f;

        [Header("References")]
        [SerializeField] private Transform playerBody;
        [SerializeField] private Transform playerCamera;


        private float xRotation = 0.0f;

        private Vector2 mouseDelta;
        private Vector2 currentDelta;
        private Vector2 smoothDelta;

        private Actions.PlayerActions playerActions;

        private void Awake()
        {
            playerActions = new Actions().Player;
        }

        private void OnEnable()
        {
            playerActions.Enable();
            playerActions.ToggleCursor.performed += OnToggleCursor;
        }

        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            if (Cursor.lockState != CursorLockMode.Locked) return;

            mouseDelta = playerActions.Look.ReadValue<Vector2>();

            // згладжування
            smoothDelta = Vector2.Lerp(smoothDelta, mouseDelta, smoothTime);
            currentDelta = smoothDelta * sensitivity;

            // Вертикальний поворот камери
            xRotation -= currentDelta.y;
            xRotation = Mathf.Clamp(xRotation, minY, maxY);

            playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

            // Горизонтальний поворот тіла
            playerBody.Rotate(Vector3.up * currentDelta.x);
        }

        private void OnDisable()
        {
            playerActions.Disable();
            playerActions.ToggleCursor.performed -= OnToggleCursor;
        }

        private void OnToggleCursor(InputAction.CallbackContext context)
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }