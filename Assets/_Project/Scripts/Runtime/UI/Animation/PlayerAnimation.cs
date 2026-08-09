using EnhancedShift.Player.Core;
using UnityEngine;

namespace EnhancedShift.Player.Animation
{
    public class PlayerAnimation : MonoBehaviour
    {
        [SerializeField] private ControllerPlayer pc;

        private static readonly int MoveXHash = Animator.StringToHash("MoveX");
        private static readonly int MoveZHash = Animator.StringToHash("MoveZ");
        private static readonly int GroundHash = Animator.StringToHash("Ground");
        private static readonly int JumpHash = Animator.StringToHash("Jump");
        private static readonly int SprintHash = Animator.StringToHash("Sprint");

        private void Awake()
        {
            if (!pc) pc = GetComponent<ControllerPlayer>();
        }

        private void Update()
        {
            if (!pc || !pc.An || pc.St == null) return;

            // Передаем состояние земли и спринта
            pc.An.SetBool(GroundHash, pc.St.Ground);
            pc.An.SetBool(SprintHash, pc.St.Run);

            // Прыжок/падение
            float jumpValue = 0f;
            if (pc.St.Jump) jumpValue = 1f;
            else if (pc.St.Fall) jumpValue = -1f;

            pc.An.SetFloat(JumpHash, jumpValue);

            // Перемещение X и Z
            Vector2 input = GetMoveInput();

            // Плавная интерполяция к значениям (или прямое присвоение)
            pc.An.SetFloat(MoveXHash, input.x);
            pc.An.SetFloat(MoveZHash, input.y);
        }

        private Vector2 GetMoveInput()
        {
            // Считываем WASD напрямую без промежуточных проверок pc.St.Walk
            float inputX = Input.GetAxisRaw("Horizontal");
            float inputZ = Input.GetAxisRaw("Vertical");

            Vector2 dir = new Vector2(inputX, inputZ);

            if (dir.sqrMagnitude < 0.01f)
                return Vector2.zero;

            dir.Normalize();
            float speedMultiplier = pc.St.Run ? 1f : 0.5f;

            return dir * speedMultiplier;
        }

        public void TriggerJump()
        {
        }
    }
}