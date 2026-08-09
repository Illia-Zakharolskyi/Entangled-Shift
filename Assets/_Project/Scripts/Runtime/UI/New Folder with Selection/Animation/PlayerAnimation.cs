using EnhancedShift.Player.Core;
using UnityEngine;

namespace EnhancedShift.Player.Animation
{
    public class PlayerAnimation : MonoBehaviour
    {
        [SerializeField] private ControllerPlayer pc;
        [SerializeField] private float dampTime = 0.1f;

        private static readonly int MoveXHash = Animator.StringToHash("MoveX");
        private static readonly int MoveZHash = Animator.StringToHash("MoveZ");
        private static readonly int SitHash = Animator.StringToHash("Crouch");
        private static readonly int FallHash = Animator.StringToHash("Fall");
        private static readonly int JumpHash = Animator.StringToHash("Jump");
        private static readonly int AttackHash = Animator.StringToHash("Attack");
        private static readonly int HurtHash = Animator.StringToHash("Hurt");
        private static readonly int DeadHash = Animator.StringToHash("Dead");

        private void Awake()
        {
            if (!pc) pc = GetComponent<ControllerPlayer>();
        }

        private void Update()
        {
            if (!pc || !pc.An) return;

            float inputX = Input.GetAxisRaw("Horizontal");
            float inputZ = Input.GetAxisRaw("Vertical");

            float multiplier = pc.St.Run ? 2f : 1f;

            float targetX = inputX * multiplier;
            float targetZ = inputZ * multiplier;

            pc.An.SetFloat(MoveXHash, targetX, dampTime, Time.deltaTime);
            pc.An.SetFloat(MoveZHash, targetZ, dampTime, Time.deltaTime);

            pc.An.SetBool(SitHash, pc.St.Sit);
            pc.An.SetBool(FallHash, pc.St.Fall);
            pc.An.SetBool(DeadHash, pc.St.Dead);
        }

        public void TriggerJump() => pc.An.SetTrigger(JumpHash);
        public void TriggerAttack() => pc.An.SetTrigger(AttackHash);
        public void TriggerHurt() => pc.An.SetTrigger(HurtHash);
    }
}