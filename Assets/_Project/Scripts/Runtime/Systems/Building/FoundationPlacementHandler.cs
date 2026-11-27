using System.Collections.Generic;
using UnityEngine;

namespace EntangledShift.Systems.Building
{
    public class FoundationPlacementHandler : PlacementHandler
    {
        [Header("References")]
        [SerializeField] private List<SocketType> _allowedSocketTypes = new() { SocketType.FoundationTriangle };
        [SerializeField] private LayerMask _mask;
        [SerializeField] private float _animationDuration;
        [SerializeField] private float _dropheight; // how high up the part spawns before it falls

        public override bool CanPlace(RaycastHit hit)
        {
            _targetSocket = null;
            BuildingSocket socket;

            Vector3 candidatePos;

            if (hit.collider.TryGetComponent<BuildingSocket>(out socket))
            {
                if (socket.IsOccupied || !_allowedSocketTypes.Contains(socket.Type))
                {
                    return false;
                }

                _targetSocket = socket;
                candidatePos = socket.transform.position;
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

            // check for ground
            if (!Physics.Raycast(candidatePos, Vector3.down, out RaycastHit groundHit, _boxSize.y, _mask))
            {
                return false;
            }

            float groundHeightDifference = candidatePos.y - groundHit.point.y;
            if (groundHeightDifference < _minGroundOffset || groundHeightDifference > _maxGroundOffset)
            {
                return false;
            }

            /* OverlapBoxNonAlloc is halfextent and _boxSize is full size. We need a division by 2 here, otherwise the overlap would be 2x size of the building.
             Also we need this little 0.01 here because it removes collision with nearby foundations - including the foundation of the socket we build on.
             we need this because physics system is not strictly accurate and when in perfect math we don't have a collision on coordinates 0,0 - 0,1,
             we have here because of how computers work with floating numbers */
            int hitCount = Physics.OverlapBoxNonAlloc(transform.position, _boxSize / 2.01f, _hitBuffer, Quaternion.identity, _data.ObstacleMask);

            if (hitCount > 0)
            {
                return false;
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

            GameObject obj = Instantiate(_data.BuildPrefab, position, rotation);
            Animate(obj, _animationDuration, _dropheight);
        }
    }
}