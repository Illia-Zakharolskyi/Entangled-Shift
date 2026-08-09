using System;
using UnityEngine;

namespace EnhancedShift.Core
{
    public class Health : MonoBehaviour
    {
        [Header("Health Settings")]
        [SerializeField] private float maxHp = 100f;
        [SerializeField] private bool godMode;

        public float CurrentHp { get; private set; }
        public float MaxHp => maxHp;
        public bool IsDead => CurrentHp <= 0f;

      
        public event Action<float, float> OnHealthChanged; 
        public event Action OnDamaged;
        public event Action OnHealed;
        public event Action OnDeath;
        public event Action OnRevive;

        private void Awake()
        {
            CurrentHp = maxHp;
        }

        public void TakeDamage(float amount)
        {
            if (godMode || IsDead || amount <= 0f)
                return;

            CurrentHp = Mathf.Max(0f, CurrentHp - amount);

            OnDamaged?.Invoke();
            OnHealthChanged?.Invoke(CurrentHp, maxHp);

            if (IsDead)
            {
                OnDeath?.Invoke();
            }
        }

        public void Heal(float amount)
        {
            if (IsDead || amount <= 0f)
                return;

            CurrentHp = Mathf.Min(maxHp, CurrentHp + amount);

            OnHealed?.Invoke();
            OnHealthChanged?.Invoke(CurrentHp, maxHp);
        }

        public void Revive()
        {
            CurrentHp = maxHp;
            OnHealthChanged?.Invoke(CurrentHp, maxHp);
            OnRevive?.Invoke();
        }
    }
}