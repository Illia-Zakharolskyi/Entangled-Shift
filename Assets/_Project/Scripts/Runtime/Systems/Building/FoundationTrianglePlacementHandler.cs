using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace EntangledShift.Systems.Building
{
    public class FoundationTrianglePlacementHandler : PlacementHandler
    {
        [Header("Settings")]
        [SerializeField] private List<SocketType> _socketTypes = new() { SocketType.FoundationTriangle };
        [SerializeField] private GameObject _model;

        private bool _customRotation = false;

        public override bool CanPlace(RaycastHit hit, out GameObject targetToConsume)
        {
            targetToConsume = null;
            _targetSocket = null;

            BuildingSocket socket = hit.collider.GetComponent<BuildingSocket>();

            if (socket != null)
            {
                if (socket.IsOccupied || !_socketTypes.Contains(socket.Type))
                {
                    return false;
                }
                _targetSocket = socket;
            }

            int hitCount = Physics.OverlapBoxNonAlloc(_model.transform.position, _boxSize / 2.01f, _hitCache, Quaternion.identity, _data.ObstacleMask);

            if (hitCount > 0)
            {
                for (int i = 0; i < hitCount; i++)
                {
                    Collider detectedCol = _hitCache[i];
                    if (detectedCol == null) continue;

                    if (detectedCol.TryGetComponent<BuildStructure>(out var obstacleStructure))
                    {
                        if (_targetSocket != null)
                        {
                            continue;
                        }
                    }

                    return false;
                }
            }

            if (hit.collider.TryGetComponent<BuildStructure>(out var structure) && _targetSocket == null)
            {
                return false;
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
                float snappedX = Mathf.Round(hit.point.x / _boxSize.x) * _boxSize.x;
                float snappedZ = Mathf.Round(hit.point.z / _boxSize.x) * _boxSize.x;

                float snappedY = hit.point.y;

                itemPreview.transform.position = new Vector3(snappedX, snappedY, snappedZ);
                if (!_customRotation) itemPreview.transform.rotation = Quaternion.identity;
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
                Gizmos.DrawWireCube(_model.transform.position, _boxSize);
        }
    }
}