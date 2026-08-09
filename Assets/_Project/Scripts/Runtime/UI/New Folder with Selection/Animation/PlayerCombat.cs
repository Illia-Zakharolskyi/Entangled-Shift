using EnhancedShift.Core;
using EnhancedShift.Player.Core;
using UnityEngine;

namespace EnhancedShift.Player.Combat
{
    public class PlayerCombat : MonoBehaviour
    {
        [Header("Core")]
        [SerializeField] private ControllerPlayer pc;

        [Header("Combat Settings")]
        [SerializeField] private WeaponType weapon = WeaponType.Hand;
        [SerializeField] private float range = 2f;
        [SerializeField] private float radius = 0.35f;
        [SerializeField] private float damage = 20f;
        [SerializeField] private float delay = 0.6f;
        [SerializeField] private LayerMask hitMask;

        private float timer;
        private bool isAttacking;

        private void Awake()
        {
            if (!pc) pc = GetComponent<ControllerPlayer>();
        }

        private void Update()
        {
            Cooldown();
            CheckInput();
        }

        private void CheckInput()
        {
            if (!pc.Down("Attack") || isAttacking || pc.St.Hurt || pc.St.Dead)
                return;

            ExecuteAttack();
        }

        private void Cooldown()
        {
            if (!isAttacking) return;

            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                isAttacking = false;
                pc.St.Atk = false;
            }
        }

        private void ExecuteAttack()
        {
            isAttacking = true;
            timer = delay;
            pc.St.Atk = true;

            
            pc.Anim(weapon.ToString());

            Vector3 origin = pc.Cam.transform.position;
            Vector3 dir = pc.Cam.transform.forward;

            if (Physics.SphereCast(origin, radius, dir, out RaycastHit hit, range, hitMask))
            {
                if (hit.collider.TryGetComponent(out IDamage target))
                {
                    target.TakeDamage(damage);
                }
            }
        }
    }
}