using UnityEngine;

namespace EntangledShift.Systems.Building
{
    public class FoundationPlacementHandler : PlacementHandler
    {
        [Header("References")]
        [SerializeField] private LayerMask _groundLayer;

        public override bool CanPlace(RaycastHit hit, out GameObject targetToConsume)
        {
            targetToConsume = null;

            if (!_groundLayer.Contains(hit.transform.gameObject.layer))
            {
                return false;
            }

            if (hit.collider.TryGetComponent<BuildStructure>(out var structure))
            {
                return false;
            }

            int hitCount = Physics.OverlapBoxNonAlloc(transform.position, _boxSize / 2.01f, _hitCache, Quaternion.identity, _data.ObstacleMask);

            if (hitCount > 0) return false;

            return true;
        }

        public override void UpdatePreview(GameObject itemPreview, RaycastHit hit, bool canPlace)
        {
            UpdatePreviewColor(canPlace);

            float snappedX = Mathf.Round(hit.point.x / _boxSize.x) * _boxSize.x;
            float snappedZ = Mathf.Round(hit.point.z / _boxSize.x) * _boxSize.x;

            float snappedY = hit.point.y;

            itemPreview.transform.position = new Vector3(snappedX, snappedY, snappedZ);
        }

        public override void Build(Vector3 position, Quaternion rotation)
        {
            Instantiate(_data.BuildPrefab, position, rotation);
        }
    }
}