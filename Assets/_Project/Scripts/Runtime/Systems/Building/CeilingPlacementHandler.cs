using UnityEngine;

namespace EntangledShift.Systems.Building
{
    public class CeilingPlacementHandler : MonoBehaviour, IPlacementHandler
    {
        [Header("References")]
        [SerializeField] private BuildableItemData _data;
        [SerializeField] private SocketType _socketType;
        [SerializeField] private LayerMask  _ceilingMask;

        private BuildingSocket _targetSocket;
        private Renderer _renderer;
        private MaterialPropertyBlock _block;
        private Camera _cam;
        private float _buildRange;

        private void Awake()
        {
            _cam = Camera.main;
            _renderer = GetComponent<Renderer>();
            _block = new MaterialPropertyBlock();
        }

        public bool CanPlace(RaycastHit hit, out GameObject targetToConsume)
        {
            targetToConsume = null;
            _targetSocket = null;

            Ray ray = _cam.ScreenPointToRay(new Vector2(Screen.width / 2f, Screen.height / 2f));
            if (Physics.Raycast(ray, _buildRange, _ceilingMask))
            {
                return false;
            }

            BuildingSocket socket = hit.collider.GetComponent<BuildingSocket>();
            if (socket == null || socket.IsOccupied || socket.Type != _socketType)
            {
                return false;
            }

            _targetSocket = socket;
            return true;
        }

        public void UpdatePreview(GameObject itemPreview, RaycastHit hit, bool canPlace)
        {
            UpdatePreviewColor(canPlace);

            if (_targetSocket != null)
            {
                Vector3 socketPos = _targetSocket.transform.position;
                Quaternion socketRot = _targetSocket.transform.rotation;

                // Сдвигаем центр потолка внутрь комнаты от верхнего края стены (например, на 2.5 метра, половину ширины)
                float halfCeilingSize = 2.5f;
                Vector3 finalPos = socketPos + (socketRot * Vector3.forward * halfCeilingSize);

                itemPreview.transform.position = finalPos;
                itemPreview.transform.rotation = socketRot;
            }
            else
            {
                itemPreview.transform.position = hit.point;
            }
        }

        public void Build(Vector3 position, Quaternion rotation)
        {
            if (_targetSocket != null)
            {
                _targetSocket.Occupy();
            }

            GameObject newCeiling = Instantiate(_data.BuildPrefab, position, rotation);
        }

        public bool Raycast(Ray ray, out RaycastHit hit, float buildRange)
        {
            _buildRange = buildRange;
            if (!Physics.Raycast(ray, out hit, buildRange, _data.AllowedSurfaces))
            {
                _targetSocket = null;
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