using EnhancedShift.Player.Core;
using UnityEngine;

namespace EnhancedShift.Player.Movement
{
    public class PlayerCrouch : MonoBehaviour
    {
        [Header("Core")]
        [SerializeField] private ControllerPlayer pc;

        [Header("Crouch")]
        [SerializeField] private float standHeight = 2f;
        [SerializeField] private float crouchHeight = 1.2f;
        [SerializeField] private float speed = 8f;
        [SerializeField] private LayerMask ceilingMask;

        private float targetHeight;
        private Vector3 targetCenter;

        private void Awake()
        {
            if (!pc)
                pc = GetComponent<ControllerPlayer>();

            targetHeight = standHeight;
            targetCenter = new Vector3(0f, standHeight * 0.5f, 0f);
        }

        private void Update()
        {
            Read();
        }

        private void FixedUpdate()
        {
            Resize();
        }

        private void Read()
        {
            if (pc.Hold("Crouch"))
            {
                pc.St.Sit = true;

                targetHeight = crouchHeight;
                targetCenter = new Vector3(0f, crouchHeight * 0.5f, 0f);

                pc.Anim("Crouch", true);
            }
            else
            {
                if (Ceiling())
                    return;

                pc.St.Sit = false;

                targetHeight = standHeight;
                targetCenter = new Vector3(0f, standHeight * 0.5f, 0f);

                pc.Anim("Crouch", false);
            }
        }
        private void Resize()
        {
            pc.Cc.height = Mathf.Lerp(
                pc.Cc.height,
                targetHeight,
                speed * Time.fixedDeltaTime);

            pc.Cc.center = Vector3.Lerp(
                pc.Cc.center,
                targetCenter,
                speed * Time.fixedDeltaTime);
        }

        private bool Ceiling()
        {
            Vector3 center = transform.position + Vector3.up * (crouchHeight * 0.5f);

            float radius = pc.Cc.radius * 0.9f;
            float dist = standHeight - crouchHeight;

            return Physics.SphereCast(
                center,
                radius,
                Vector3.up,
                out _,
                dist,
                ceilingMask);
        }

        private void OnDrawGizmosSelected()
        {
            if (pc == null || pc.Cc == null)
                return;

            Gizmos.color = Color.yellow;

            Vector3 center = transform.position + Vector3.up * (crouchHeight * 0.5f);
            float radius = pc.Cc.radius * 0.9f;
            float dist = standHeight - crouchHeight;

            Gizmos.DrawWireSphere(center, radius);
            Gizmos.DrawLine(center, center + Vector3.up * dist);
        }
    }
}