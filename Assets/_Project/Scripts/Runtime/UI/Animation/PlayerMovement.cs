using EnhancedShift.Player.Core;
using UnityEngine;

namespace EnhancedShift.Player.Movement
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Core")]
        [SerializeField] private ControllerPlayer pc;

        [Header("Move")]
        [SerializeField] private float walkSpeed = 3f;
        [SerializeField] private float runSpeed = 6f;
        [SerializeField] private float airSpeed = 2f;
        [SerializeField] private float rotSpeed = 20f;
        [SerializeField] private float atkSlow = 0.5f;
        [SerializeField] private float hurtSlow = 0.4f;
        [SerializeField] private float groundDrag = 5f;
        [SerializeField] private float airDrag = 0f;

        private Vector3 inputDir;
        private float speed;

        private void Awake()
        {
            if (!pc) pc = GetComponent<ControllerPlayer>();
        }

        private void Update()
        {
            ReadInput();
        }

        private void FixedUpdate()
        {
            Drag();
            RotateToCamera();
            Move();
        }

        private void ReadInput()
        {
            inputDir.x = Input.GetAxisRaw("Horizontal");
            inputDir.z = Input.GetAxisRaw("Vertical");
            inputDir = Vector3.ClampMagnitude(inputDir, 1f);

            pc.St.Walk = inputDir.sqrMagnitude > 0.01f;
            pc.St.Run = pc.Hold("Run") && inputDir.z > 0f;

            speed = pc.St.Run ? runSpeed : walkSpeed;

            if (pc.St.Jump) speed = airSpeed;
            if (pc.St.Atk) speed *= atkSlow;
            if (pc.St.Hurt) speed *= hurtSlow;
        }

        private void RotateToCamera()
        {
            if (pc.Cam == null) return;

            Vector3 camForward = pc.Cam.transform.forward;
            camForward.y = 0f;

            if (camForward.sqrMagnitude < 0.001f) return;

            Quaternion targetRotation = Quaternion.LookRotation(camForward);

            pc.Rb.MoveRotation(
                Quaternion.Slerp(
                    pc.Rb.rotation,
                    targetRotation,
                    rotSpeed * Time.fixedDeltaTime));
        }

        private void Move()
        {
            if (pc.Cam == null) return;

            Vector3 camForward = pc.Cam.transform.forward;
            Vector3 camRight = pc.Cam.transform.right;

            camForward.y = 0f;
            camRight.y = 0f;

            camForward.Normalize();
            camRight.Normalize();

            Vector3 moveDir = camForward * inputDir.z + camRight * inputDir.x;

            if (pc.St.Jump)
            {

                Vector3 velocity = pc.Rb.linearVelocity;
                velocity.y = pc.Rb.linearVelocity.y;

                pc.Rb.linearVelocity = velocity;
                return;
            }

            Vector3 targetVelocity = moveDir.normalized * speed;
            targetVelocity.y = pc.Rb.linearVelocity.y;

            pc.Rb.linearVelocity = targetVelocity;
        }

        private void Drag()
        {
            pc.Rb.linearDamping = pc.St.Ground ? groundDrag : airDrag;
        }
    }
}