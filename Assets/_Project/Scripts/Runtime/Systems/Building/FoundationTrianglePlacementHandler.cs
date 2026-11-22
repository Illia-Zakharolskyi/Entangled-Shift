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
        [SerializeField] private float _animationDuration;
        [SerializeField] private float _dropheight; // how high up the part spawns before it falls
        [SerializeField] private float _minDistance; // min distance between triangle and cube

        private bool _customRotation = false;
        private BuildingSocket _socket;
        private BuildStructure _structure;
        private bool _isBuildStructure;

        public override bool CanPlace(RaycastHit hit, out GameObject targetToConsume)
        {
            targetToConsume = null;
            _targetSocket = null;
            Vector3 candidatePos;

            if (hit.collider.TryGetComponent<BuildingSocket>(out _socket))
            {
                if (_socket.IsOccupied || !_socketTypes.Contains(_socket.Type))
                {
                    return false;
                }

                _targetSocket = _socket;
                candidatePos = _socket.transform.position;
            }
            else
            {
                candidatePos = GetSnappedPosition(hit.point);
                float heightDifference = candidatePos.y - hit.point.y;

                if (heightDifference < _minGroundOffset || heightDifference > _maxGroundOffset)
                {
                    return false;
                }
            }

            //check for ground
            if (!Physics.Raycast(candidatePos, Vector3.down, out RaycastHit groundHit, _boxSize.y, _mask))
            {
                return false;
            }

            float groundHeightDifference = candidatePos.y - groundHit.point.y;
            if (groundHeightDifference < _minGroundOffset || groundHeightDifference > _maxGroundOffset)
            {
                return false;
            }

            int hitCount = Physics.OverlapBoxNonAlloc(transform.position, _boxSize / 2f, _hitBuffer, Quaternion.identity, _data.ObstacleMask);

            if (hitCount > 0)
            {
                foreach (Collider col in _hitBuffer)
                {
                    if (col == null) continue;
                    _isBuildStructure = col.TryGetComponent<BuildStructure>(out _structure);

                    if (_isBuildStructure && _targetSocket == null)
                    {
                        return false;
                    }

                    if (_isBuildStructure && _targetSocket != null)
                    {
                        if (Vector3.Distance(candidatePos, col.transform.position) < _minDistance)
                        {
                            return false;
                        }
                    }
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
            Animate(newFoundation, _animationDuration, _dropheight);
        }

        public void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            if (Application.isPlaying)
            {
                Gizmos.DrawWireCube(transform.position, _boxSize);
            }
        }

        public static float NormalizedDistance(Vector3 a, Vector3 b, float maxDistance)
        {
            if (maxDistance <= 0f)
                return 0f;

            float sqrDist = (a - b).sqrMagnitude;
            float sqrMax = maxDistance * maxDistance;
            return Mathf.Clamp01(sqrDist / sqrMax); // Note: scales with distance^2, not linear distance
        }
    }
}