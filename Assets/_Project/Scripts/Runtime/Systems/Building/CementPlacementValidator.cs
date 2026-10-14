using System;
using UnityEngine;
using static UnityEngine.Analytics.IAnalytic;

namespace EntangledShift.Systems.Building
{
    public class CementPlacementHandler : MonoBehaviour, IPlacementHandler
    {
        [Header("Refrences")]
        [SerializeField] private BuildableItemData _data;
        private Renderer _renderer;
        private MaterialPropertyBlock _block;

        private void Awake()
        {
            _renderer = GetComponent<Renderer>();
            _block = new MaterialPropertyBlock();
        }

        public bool CanPlace(RaycastHit hit, out GameObject objectConsume)
        {
            objectConsume = null;

            return true;
        }

        public void Build(Vector3 position, Quaternion rotation)
        {
            Instantiate(_data.BuildPrefab, position, rotation);
        }

        public void UpdatePreview(GameObject itemPreview, RaycastHit hit, bool canPlace)
        {
            UpdatePreviewColor(canPlace);
            itemPreview.transform.position = hit.point;
        }

        public bool Raycast(Ray ray, out RaycastHit hit, float buildRange)
        {
            if (!Physics.Raycast(ray, out hit, buildRange, _data.AllowedSurfaces))
            {
                return false;
            }

            return true;
        }

        public void UpdatePreviewColor(bool canBuild)
        {
            Color curr = canBuild ? _data.ValidColor : _data.InvalidColor;

            _renderer.GetPropertyBlock(_block);
            _block.SetColor(_data.BaseColorId, curr);
            _renderer.SetPropertyBlock(_block);
        }
    }
}