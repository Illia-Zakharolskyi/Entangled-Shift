using UnityEngine;
using UnityEngine.InputSystem;

public class Hammer : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private LayerMask _mask;
    [SerializeField] private float _range;

    [Header("Outline Visuals")]
    [SerializeField] private Material _outlineMaterial;

    private Ray _ray;
    private RaycastHit _hit;
    private Vector3 _cameraRay;
    private Camera _camera;

    private Transform _currentTarget;
    private GameObject _currentOutlineObj;

    private void Awake()
    {
        _cameraRay = new Vector3(Screen.width / 2f, Screen.height / 2f);
        _camera = Camera.main;
    }

    private void Update()
    {
        _ray = _camera.ScreenPointToRay(_cameraRay);

        Transform newTarget = null;

        if (Physics.Raycast(_ray, out _hit, _range, _mask))
        {
            newTarget = _hit.transform;

            if (Keyboard.current.rKey.wasPressedThisFrame)
            {
                ClearOutline();
                Destroy(_hit.collider.gameObject);
                return;
            }
        }

        if (newTarget != _currentTarget)
        {
            ClearOutline();
            _currentTarget = newTarget;

            if (_currentTarget != null)
            {
                CreateOutline(_currentTarget);
            }
        }
    }

    private void CreateOutline(Transform target)
    {
        MeshFilter targetMeshFilter = target.GetComponent<MeshFilter>();
        if (targetMeshFilter == null || targetMeshFilter.sharedMesh == null)
        {
            return;
        }

        _currentOutlineObj = new GameObject("VisualOutline");
        _currentOutlineObj.transform.SetParent(target, false);
        _currentOutlineObj.transform.localPosition = Vector3.zero;
        _currentOutlineObj.transform.localRotation = Quaternion.identity;
        _currentOutlineObj.transform.localScale = Vector3.one;

        MeshFilter mf = _currentOutlineObj.AddComponent<MeshFilter>();
        mf.sharedMesh = targetMeshFilter.sharedMesh;

        MeshRenderer mr = _currentOutlineObj.AddComponent<MeshRenderer>();
        mr.sharedMaterial = _outlineMaterial;
    }

    private void ClearOutline()
    {
        if (_currentOutlineObj != null)
        {
            Destroy(_currentOutlineObj);
            _currentOutlineObj = null;
        }
        _currentTarget = null;
    }
}