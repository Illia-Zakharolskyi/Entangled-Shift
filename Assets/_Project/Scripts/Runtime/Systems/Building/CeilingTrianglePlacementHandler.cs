using System.Collections.Generic;
using UnityEngine;

namespace EntangledShift.Systems.Building
{
    public class CeilingTrianglePlacementHandler : PlacementHandler
    {
        [Header("Settings")]
        [SerializeField] private List<SocketType> _socketTypes = new() { SocketType.WallTopInner };

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
            if (wallStructure == null || wallStructure.Shape != StructureShape.Triangle)
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
                Vector3 socketPos = _targetSocket.transform.position;
                Quaternion socketRot = _targetSocket.transform.rotation;

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

        public override void Build(Vector3 position, Quaternion rotation)
        {
            if (_targetSocket != null)
            {
                _targetSocket.Occupy();
            }

            GameObject newCeiling = Instantiate(_data.BuildPrefab, position, rotation);
        }
    }
}