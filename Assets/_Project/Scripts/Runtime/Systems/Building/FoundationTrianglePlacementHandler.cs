using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace EntangledShift.Systems.Building
{
    public class FoundationTrianglePlacementHandler : PlacementHandler
    {
        [Header("Settings")]
        [SerializeField] private List<SocketType> _socketTypes = new() { SocketType.FoundationTriangle };
        [SerializeField] private LayerMask _mask;
        [SerializeField] private GameObject _model;

        private bool _customRotation = false;

        public override bool CanPlace(RaycastHit hit, out GameObject targetToConsume)
        {
            targetToConsume = null;
            _targetSocket = null;

            if (_mask.Contains(hit.transform.gameObject.layer) &&
                !hit.collider.TryGetComponent<BuildingSocket>(out _))
            {
                return false;
            }

            if (hit.collider.TryGetComponent<BuildingSocket>(out var socket))
            {
                if (socket.IsOccupied || !_socketTypes.Contains(socket.Type) || socket.Shape != StructureShape.Triangle)
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

            int hitCount = Physics.OverlapBoxNonAlloc(_model.transform.position, _boxSize / 2.01f, _hitBuffer, Quaternion.identity, _data.ObstacleMask);

            if (hitCount > 0)
            {
                for (int i = 0; i < hitCount; i++)
                {
                    Collider detectedCol = _hitBuffer[i];
                    if (detectedCol == null)
                    {
                        continue;
                    }

                    if (detectedCol.TryGetComponent<BuildStructure>(out _))
                    {
                        if (_targetSocket != null)
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

            if (Keyboard.current.rKey.wasPressedThisFrame && _targetSocket == null)
            {
                itemPreview.transform.Rotate(0f, 90f, 0f, Space.World);
                _customRotation = true;
            }

            if (_targetSocket != null)
            {
                itemPreview.transform.position = _targetSocket.transform.position;
                itemPreview.transform.rotation = _targetSocket.transform.rotation;
                itemPreview.transform.localScale = Vector3.one;
            }
            else
            {
                itemPreview.transform.position = GetSnappedPosition(hit.point);
                if (!_customRotation)
                {
                    itemPreview.transform.rotation = Quaternion.identity;
                }
            }
        }

        public override void Build(Vector3 position, Quaternion rotation)
        {
            if (_targetSocket != null)
            {
                _targetSocket.Occupy();
            }

            GameObject newFoundation = Instantiate(_data.BuildPrefab, position, rotation);

            newFoundation.transform.localScale = Vector3.one;
        }

        public void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            if (Application.isPlaying)
            {
                Gizmos.DrawWireCube(_model.transform.position, _boxSize);
            }
        }
    }
}