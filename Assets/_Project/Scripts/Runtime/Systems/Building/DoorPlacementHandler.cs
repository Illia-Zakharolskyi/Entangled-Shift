using EntangledShift.Systems.Building;
using UnityEngine;

namespace EntangledShift
{
    public class DoorPlacementHandler : MonoBehaviour, IPlacementHandler
    {
        [Header("References")]
        [SerializeField] private BuildableItemData _data;

        private BuildingSocket _targetSocket;
        private Renderer _renderer;
        private MaterialPropertyBlock _block;

        private void Awake()
        {
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
            _targetSocket = null;

            BuildingSocket socket = hit.collider.GetComponent<BuildingSocket>();
            if (socket == null || socket.IsOccupied || socket.Type != SocketType.WallDoorway)
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
                itemPreview.transform.position = _targetSocket.transform.position;
                itemPreview.transform.rotation = _targetSocket.transform.rotation;
                itemPreview.transform.localScale = Vector3.one;
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

            GameObject newWall = Instantiate(_data.BuildPrefab, position, rotation);
            newWall.transform.localScale = Vector3.one;
        }

        public bool Raycast(Ray ray, out RaycastHit hit, float buildRange)
        {
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
