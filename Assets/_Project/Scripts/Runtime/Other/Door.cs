using DG.Tweening;
using UnityEngine;

namespace EntangledShift
{
    public class Door : MonoBehaviour, IMainInteractable
    {
        [Header("Settings")]
        [SerializeField] private float _openAngle = 90f;
        [SerializeField] private float _animationDuration = 0.5f;
        [SerializeField] private Ease _animationEase = Ease.OutQuad;
        [SerializeField] private LayerMask _playerLayer;

        private bool _isOpen = false;
        private bool _isAnimating = false;

        private Quaternion _closedRotation;
        private Quaternion _openRotation;

        private void Start()
        {
            _closedRotation = transform.localRotation;
        }

        public string GetPrompt()
        {
            if (_isAnimating) return string.Empty;
            return _isOpen ? "Close" : "Open";
        }

        public void MainInteract()
        {
            if (_isAnimating) return;

            ToggleDoor();
        }

        private void ToggleDoor()
        {
            if (_isAnimating) return;

            _isOpen = !_isOpen;
            _isAnimating = true;

            float targetAngle = _openAngle;

            // Ищем игрока по слою в радиусе взаимодействия с помощью OverlapSphere
            Collider[] hits = Physics.OverlapSphere(transform.position, 5, _playerLayer);
            if (hits.Length > 0)
            {
                Transform player = hits[0].transform;
                
                // Проверяем, с какой стороны от двери находится игрок (через скалярное произведение)
                Vector3 toPlayer = player.position - transform.position;
                float dot = Vector3.Dot(transform.forward, toPlayer);

                // Если игрок сзади двери, инвертируем угол, чтобы дверь открывалась наружу/внутрь
                if (dot < 0)
                {
                    targetAngle = -_openAngle;
                }
            }

            // Рассчитываем конечный поворот относительно закрытого состояния
            Quaternion targetRotation = _closedRotation * Quaternion.Euler(0, _isOpen ? targetAngle : 0, 0);

            // Запускаем анимацию через DOTween
            transform.DOLocalRotateQuaternion(targetRotation, _animationDuration)
                .SetEase(_animationEase)
                .OnComplete(() => {
                    _isAnimating = false;
                });
        }
    }
}
