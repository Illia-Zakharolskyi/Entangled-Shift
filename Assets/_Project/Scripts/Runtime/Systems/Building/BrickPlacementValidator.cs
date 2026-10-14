using UnityEngine;

namespace EntangledShift.Systems.Building
{
    public class BrickPlacementHandler : MonoBehaviour, IPlacementHandler
    {
        [Header("References")]
        [SerializeField] private BuildableItemData _data;

        private WetCement _cement;
        private Renderer _renderer;
        private MaterialPropertyBlock _block;

        private void Awake()
        {
            _renderer = GetComponent<Renderer>();
            _block = new MaterialPropertyBlock();
        }

        public bool CanPlace(RaycastHit hit, out GameObject targetToConsume)
        {
            targetToConsume = null;
            _cement = null;

            _cement = hit.collider.GetComponentInParent<WetCement>();
            if (_cement == null) return false;

            targetToConsume = _cement.gameObject;
            return true;
        }

        public void UpdatePreview(GameObject itemPreview, RaycastHit hit, bool canBuild)
        {
            UpdatePreviewColor(canBuild);

            if (_cement != null)
            {
                float cementTopY = _cement.GetComponentInChildren<Collider>().bounds.min.y;
                itemPreview.transform.position = new Vector3(hit.point.x, cementTopY, hit.point.z);
            }
            else
            {
                itemPreview.transform.position = hit.point;
            }
        }

        public void Build(Vector3 position, Quaternion rotation)
        {
            Instantiate(_data.BuildPrefab, position, rotation);
        }

        public bool Raycast(Ray ray, out RaycastHit hit, float buildRange)
        {
            if (!Physics.Raycast(ray, out hit, buildRange, _data.AllowedSurfaces))
            {
                _cement = null;
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