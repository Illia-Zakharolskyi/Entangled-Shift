using System.Collections;
using EnhancedShift.Player.Core;
using UnityEngine;

namespace EnhancedShift.Player.Movement
{
    public class PlayerJump : MonoBehaviour
    {
        [Header("Core")]
        [SerializeField] private ControllerPlayer pc;

        [Header("Ground Check")]
        [SerializeField] private LayerMask groundMask;
        [SerializeField] private float groundCheckRadius = 0.25f;
        [SerializeField] private Vector3 groundCheckOffset = new Vector3(0, -0.9f, 0);

        [Header("Jump Settings")]
        [SerializeField] private float jumpForce = 7f;
        [SerializeField] private float jumpCooldown = 0.15f;

        [Header("Collider Pinch Settings")]
        [SerializeField] private float normalHeight = 2f;
        [SerializeField] private float jumpHeight = 1.4f;
        [SerializeField] private Vector3 normalCenter = new Vector3(0, 1f, 0);
        [SerializeField] private float resizeSpeed = 15f;

        private bool ground;
        private bool isJumping;

        private float targetHeight;
        private Vector3 targetCenter;

        private static readonly int JumpHash = Animator.StringToHash("Jump");
        private static readonly int GroundHash = Animator.StringToHash("Ground");

        private void Awake()
        {
            if (!pc) pc = GetComponent<ControllerPlayer>();
        }

        private void Start()
        {
            targetHeight = normalHeight;
            targetCenter = normalCenter;
        }

        private void Update()
        {
            CheckGround();
            Jump();
            UpdateCollider();
            UpdateAnimation();
        }

        private void CheckGround()
        {
            if (!pc) return;

            if (!isJumping)
            {
                Vector3 spherePosition = transform.position + groundCheckOffset;
                ground = Physics.CheckSphere(spherePosition, groundCheckRadius, groundMask);
                pc.St.Ground = ground;
            }

            if (ground)
            {
                pc.St.Jump = false;
                pc.St.Fall = false;

                targetHeight = normalHeight;
                targetCenter = normalCenter;
            }
            else
            {
                if (pc.Rb != null && pc.Rb.linearVelocity.y < -0.1f)
                {
                    pc.St.Fall = true;
                }

                targetHeight = jumpHeight;
                targetCenter = new Vector3(normalCenter.x, normalCenter.y + (normalHeight - jumpHeight) * 0.5f, normalCenter.z);
            }
        }

        private void Jump()
        {
            if (!ground || isJumping) return;
            if (!pc.Down("Jump")) return;

            StartCoroutine(JumpRoutine());
        }

        private IEnumerator JumpRoutine()
        {
            isJumping = true;
            ground = false;

            pc.St.Ground = false;
            pc.St.Jump = true;

            if (pc.Rb != null)
            {
               
                Vector3 vel = pc.Rb.linearVelocity;
                vel.y = jumpForce;
                pc.Rb.linearVelocity = vel;
            }

            yield return new WaitForSeconds(jumpCooldown);

            isJumping = false;
        }

        private void UpdateCollider()
        {
            if (pc != null && pc.Cc != null)
            {
                pc.Cc.height = Mathf.Lerp(pc.Cc.height, targetHeight, Time.deltaTime * resizeSpeed);
                pc.Cc.center = Vector3.Lerp(pc.Cc.center, targetCenter, Time.deltaTime * resizeSpeed);
            }
        }

        private void UpdateAnimation()
        {
            if (pc && pc.An)
            {
                pc.An.SetBool(GroundHash, pc.St.Ground);
                if (pc.Rb != null)
                {
                    pc.An.SetFloat(JumpHash, pc.Rb.linearVelocity.y);
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position + groundCheckOffset, groundCheckRadius);
        }
    }
}