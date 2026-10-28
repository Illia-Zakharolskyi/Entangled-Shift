using System.Collections.Generic;
using UnityEngine;

namespace EntangledShift.Systems.Building
{
    public class WallPlacementHandler : PlacementHandler
    {
        [Header("Settings")]
        [SerializeField] private List<SocketType> _socketTypes = new() { SocketType.FoundationTop, SocketType.WallTopOutside };

        public override bool CanPlace(RaycastHit hit, out GameObject targetToConsume)
        {
            targetToConsume = null;
            _targetSocket = null;

            if (hit.collider.TryGetComponent<BuildStructure>(out var structure) && structure.Data == _data)
            {
                return false;
            }

            BuildingSocket socket = hit.collider.GetComponent<BuildingSocket>();

            if (socket == null || socket.IsOccupied || !_socketTypes.Contains(socket.Type))
            {
                return false;
            }

            BuildStructure wallStructure = socket.transform.root.GetComponentInChildren<BuildStructure>();
            if (wallStructure == null || wallStructure.Shape != StructureShape.Cube && socket.Type != SocketType.WallTopOutside)
            {
                return false;
            }

            _targetSocket = socket;
            return true;
        }

        public override void UpdatePreview(GameObject itemPreview, RaycastHit hit, bool canPlace)
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

        public override void Build(Vector3 position, Quaternion rotation)
        {
            if (_targetSocket != null)
            {
                _targetSocket.Occupy();
            }

            GameObject newWall = Instantiate(_data.BuildPrefab, position, rotation);
            newWall.transform.localScale = Vector3.one;

            if (_targetSocket != null)
            {
                BuildStructure wallStructure = newWall.GetComponentInChildren<BuildStructure>();
                if (wallStructure != null)
                {
                    wallStructure.Shape = _targetSocket.Shape;
                }
            }
        }
    }
}