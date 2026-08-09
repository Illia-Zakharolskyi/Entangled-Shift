using EnhancedShift.Player.Core;
using UnityEngine;

namespace EnhancedShift.Player.Interaction
{
    public class PlayerInteraction : MonoBehaviour
    {
        [Header("Core")]
        [SerializeField] private ControllerPlayer pc;

        [Header("Interact")]
        [SerializeField] private float distance = 2f;
        [SerializeField] private LayerMask mask;

        private RaycastHit hit;
        private IInteract target;

        private void Awake()
        {
            if (!pc)
                pc = GetComponent<ControllerPlayer>();
        }

        private void Update()
        {
            Scan();
            Input();
        }

        private void Scan()
        {
            target = null;

            if (pc.Cam == null)
                return;

            if (!Physics.Raycast(
                    pc.Cam.transform.position,
                    pc.Cam.transform.forward,
                    out hit,
                    distance,
                    mask))
                return;

            hit.collider.TryGetComponent(out target);
        }

        private void Input()
        {
            if (target == null)
                return;

            if (!pc.Down("Interact"))
                return;

            target.Interact();

            pc.St.Pick = true;
            pc.Anim("Pick");
        }
        private void LateUpdate()
        {
            if (!pc.St.Pick)
                return;

            pc.St.Pick = false;
        }

        private void OnDrawGizmosSelected()
        {
            if (pc == null || pc.Cam == null)
                return;

            Gizmos.color = Color.cyan;

            Gizmos.DrawRay(
                pc.Cam.transform.position,
                pc.Cam.transform.forward * distance);
        }

        public IInteract GetTarget()
        {
            return target;
        }

        public RaycastHit GetHit()
        {
            return hit;
        }

        public bool HasTarget()
        {
            return target != null;
        }
    }
}