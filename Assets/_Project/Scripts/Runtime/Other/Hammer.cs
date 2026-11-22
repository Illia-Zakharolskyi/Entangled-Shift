using EntangledShift.Runtime.Systems;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace EntangledShift.Runtime
{
    public class Hammer : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private LayerMask _mask;
        [SerializeField] private float _range;
        [SerializeField] AudioClip _clip;

        [Header("Outline Visuals")]
        [SerializeField] private Material _outlineMaterial;

        private Ray _ray;
        private RaycastHit _hit;
        private Vector3 _cameraRay;
        private Camera _camera;

        private Transform _currentTarget;
        private readonly List<GameObject> _currentOutlineObjs = new();

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
                    if (AudioController.Instance) AudioController.Instance.PlayOneShotSFXSound(_clip);
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
            if (target.parent != null)
            {
                MeshFilter[] parentMeshFilters = target.parent.GetComponentsInChildren<MeshFilter>();
                if (parentMeshFilters.Length > 1)
                {
                    foreach (MeshFilter mf in parentMeshFilters)
                    {
                        if (mf == null || mf.sharedMesh == null || mf.gameObject.name == "VisualOutline")
                        {
                            continue;
                        }

                        _currentOutlineObjs.Add(CreateSingleOutline(mf.transform, mf.sharedMesh));
                    }
                    return;
                }
            }

            MeshFilter targetMeshFilter = target.GetComponent<MeshFilter>();
            if (targetMeshFilter == null || targetMeshFilter.sharedMesh == null)
            {
                return;
            }

            _currentOutlineObjs.Add(CreateSingleOutline(target, targetMeshFilter.sharedMesh));
        }

        private GameObject CreateSingleOutline(Transform parentTarget, Mesh sharedMesh)
        {
            GameObject outlineObj = new GameObject("VisualOutline");
            outlineObj.transform.SetParent(parentTarget, false);
            outlineObj.transform.localPosition = Vector3.zero;
            outlineObj.transform.localRotation = Quaternion.identity;
            outlineObj.transform.localScale = Vector3.one;

            MeshFilter mf = outlineObj.AddComponent<MeshFilter>();
            mf.sharedMesh = sharedMesh;

            MeshRenderer mr = outlineObj.AddComponent<MeshRenderer>();
            mr.sharedMaterial = _outlineMaterial;

            return outlineObj;
        }

        private void ClearOutline()
        {
            for (int i = 0; i < _currentOutlineObjs.Count; i++)
            {
                if (_currentOutlineObjs[i] != null)
                {
                    Destroy(_currentOutlineObjs[i]);
                }
            }
            _currentOutlineObjs.Clear();
            _currentTarget = null;
        }
    }
}