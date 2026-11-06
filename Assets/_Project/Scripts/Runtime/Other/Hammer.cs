using UnityEngine;
using UnityEngine.InputSystem;

namespace EntangledShift
{
    public class Hammer : MonoBehaviour
    {
        [SerializeField] private LayerMask _mask;
        [SerializeField] private float _range;

        private Ray _ray;
        private RaycastHit _hit;
        private Vector3 _cameraRay;
        private Camera _camera;


        private void Awake()
        {
            _cameraRay = new Vector3(Screen.height / 2f, Screen.width / 2f);
            _camera = Camera.main;
        }

        private void Update()
        {
            _ray = _camera.ScreenPointToRay(_cameraRay);

            if (Physics.Raycast(_ray, out _hit, _range, _mask))
            {
                if (Keyboard.current.rKey.wasPressedThisFrame)
                {
                    Destroy(_hit.collider.gameObject);
                }
            }
        }
    }
}
