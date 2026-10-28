using UnityEngine;

namespace EntangledShift.Systems.Building
{
    public class StairPlacementHandler : PlacementHandler
    {
        public override bool CanPlace(RaycastHit hit, out GameObject targetToConsume)
        {
            targetToConsume = null;

            if (hit.collider.TryGetComponent<BuildStructure>(out var structure) && structure.Data == _data)
            {
                return false;
            }

            return true;

        }

        public override void UpdatePreview(GameObject itemPreview, RaycastHit hit, bool canPlace)
        {
            UpdatePreviewColor(canPlace);

            itemPreview.transform.position = hit.point;
            itemPreview.transform.localScale = Vector3.one;
        }

        public override void Build(Vector3 position, Quaternion rotation)
        {
            GameObject newStairs = Instantiate(_data.BuildPrefab, position, rotation);
            newStairs.transform.localScale = Vector3.one;
        }
    }
}