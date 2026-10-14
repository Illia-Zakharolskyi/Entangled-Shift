using UnityEngine;

namespace EntangledShift.Systems.Building
{
    public class FoundationPlacementHandler : MonoBehaviour, IPlacementHandler
    {
        [Header("References")]
        [SerializeField] private BuildableItemData _data;
        [SerializeField] private LayerMask _foundationLayer;

        [Header("Settings")]
        [SerializeField] private float _gridSize;
        private Renderer _renderer;
        private MaterialPropertyBlock _block;
        private Camera _cam;
        private float _buildRange;

        private void Awake()
        {
            _cam = Camera.main;
            _renderer = GetComponent<Renderer>();
            if (_renderer == null)
            {
                _renderer = GetComponentInChildren<Renderer>();
            }
            _block = new MaterialPropertyBlock();
        }

        public bool CanPlace(RaycastHit hit, out GameObject targetToConsume)
        {
            targetToConsume = null;

            Ray ray = _cam.ScreenPointToRay(new Vector2(Screen.width / 2f, Screen.height / 2f));
            if (Physics.Raycast(ray, _buildRange, _foundationLayer))
            {
                return false;
            }

            return true;
        }

        public void UpdatePreview(GameObject itemPreview, RaycastHit hit, bool canPlace)
        {
            UpdatePreviewColor(canPlace);

            float snappedX = Mathf.Round(hit.point.x / _gridSize) * _gridSize;
            float snappedZ = Mathf.Round(hit.point.z / _gridSize) * _gridSize;

            float snappedY = hit.point.y;

            itemPreview.transform.position = new Vector3(snappedX, snappedY, snappedZ);
        }

        public void Build(Vector3 position, Quaternion rotation)
        {
            Instantiate(_data.BuildPrefab, position, rotation);
        }

        public bool Raycast(Ray ray, out RaycastHit hit, float buildRange)
        {
            _buildRange = buildRange;
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