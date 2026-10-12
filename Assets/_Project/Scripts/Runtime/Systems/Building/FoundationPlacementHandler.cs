using UnityEngine;

namespace EntangledShift.Systems.Building
{
    public class FoundationPlacementHandler : MonoBehaviour, IPlacementHandler
    {
        [Header("References")]
        [SerializeField] private BuildableItemData _foundationData;

        [Header("Settings")]
        [SerializeField] private float _gridSize;

        public bool CanPlace(RaycastHit hit, out GameObject targetToConsume)
        {
            targetToConsume = null;
            if (!_foundationData.AllowedSurfaces.Contains(hit.transform.gameObject.layer))
            {
                return false;
            }

            return true;
        }

        public void UpdatePreview(GameObject itemPreview, RaycastHit hit, bool canPlace)
        {
            float snappedX = Mathf.Round(hit.point.x / _gridSize) * _gridSize;
            float snappedZ = Mathf.Round(hit.point.z / _gridSize) * _gridSize;
            
            float snappedY = hit.point.y;

            itemPreview.transform.position = new Vector3(snappedX, snappedY, snappedZ);
        }

        public void Build(Vector3 position, Quaternion rotation)
        {
            Instantiate(_foundationData.BuildPrefab, position, rotation);
        }
    }
}