using System.Collections.Generic;
using UnityEngine;

namespace EntangledShift.Systems.Building
{
    public class FoundationPlacementHandler : PlacementHandler
    {
        [Header("References")]
        [SerializeField] private List<SocketType> _allowedSocketTypes = new() { SocketType.FoundationTriangle };
        [SerializeField] private LayerMask _mask;

        public override bool CanPlace(RaycastHit hit, out GameObject targetToConsume)
        {
            targetToConsume = null;
            _targetSocket = null;
            BuildingSocket socket;

            if (!_mask.Contains(hit.transform.gameObject.layer) &&
                !hit.collider.TryGetComponent<BuildingSocket>(out _))
            {
                return false;
            }

            if (hit.collider.TryGetComponent<BuildingSocket>(out socket))
            {
                if (socket.IsOccupied || !_allowedSocketTypes.Contains(socket.Type) || socket.Shape != StructureShape.Square)
                {
                    return false;
                }

                _targetSocket = socket;
            }
            else
            {
                Vector3 targetPos = GetSnappedPosition(hit.point);
                float heightDifference = targetPos.y - hit.point.y;

                if (heightDifference < _minGroundOffset || heightDifference > _maxGroundOffset)
                {
                    return false;
                }
            }

            int hitCount = Physics.OverlapBoxNonAlloc(transform.position, _boxSize / 2.01f, _hitBuffer, Quaternion.identity, _data.ObstacleMask);

            if (hitCount > 0)
            {
                for (int i = 0; i < hitCount; i++)
                {
                    if (_hitBuffer[i] != null && _targetSocket != null)
                    {
                        if (_hitBuffer[i].transform.root == _targetSocket.transform.root)
                        {
                            continue;
                        }
                    }
                    return false;
                }
            }

            return true;
        }

        public override void UpdatePreview(GameObject itemPreview, RaycastHit hit, bool canPlace)
        {
            UpdatePreviewColor(canPlace);

            if (_targetSocket != null)
            {
                itemPreview.transform.position = _targetSocket.transform.position;
                itemPreview.transform.rotation = _targetSocket.transform.rotation;
            }
            else
            {
                itemPreview.transform.position = GetSnappedPosition(hit.point);
                itemPreview.transform.rotation = Quaternion.identity;
            }
        }

        public override void Build(Vector3 position, Quaternion rotation)
        {
            if (_targetSocket != null)
            {
                _targetSocket.Occupy();
            }

            _ = Instantiate(_data.BuildPrefab, position, rotation);
        }
    }
}