using EnhancedShift.Core;
using EnhancedShift.Player.Core;
using UnityEngine;

namespace EnhancedShift.Player.Health
{
    [RequireComponent(typeof(EnhancedShift.Core.Health))]
    public class PlayerHealth : MonoBehaviour, IDamage
    {
        [SerializeField] private ControllerPlayer pc;
        private EnhancedShift.Core.Health health;

        private void Awake()
        {
            if (!pc) pc = GetComponent<ControllerPlayer>();
            health = GetComponent<EnhancedShift.Core.Health>();
        }

        private void OnEnable()
        {
            health.OnDamaged += HandleDamaged;
            health.OnDeath += HandleDeath;
            health.OnRevive += HandleRevive;
        }

        private void OnDisable()
        {
            health.OnDamaged -= HandleDamaged;
            health.OnDeath -= HandleDeath;
            health.OnRevive -= HandleRevive;
        }

        public void TakeDamage(float damage) => health.TakeDamage(damage);

        private void HandleDamaged()
        {
            pc.St.Hurt = true;
        }

        private void HandleDeath()
        {
            pc.St.Stop(); 
            pc.Rb.linearVelocity = Vector3.zero;
        }

        private void HandleRevive()
        {
            pc.St.Dead = false;
            pc.St.Hurt = false;
        }
    }
}