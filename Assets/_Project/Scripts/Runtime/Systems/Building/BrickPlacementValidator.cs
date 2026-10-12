using UnityEngine;

namespace EntangledShift.Systems.Building
{
    public class BrickPlacementHandler : MonoBehaviour, IPlacementHandler
    {
        [Header("References")]
        [SerializeField] private BuildableItemData _brickData;

        private WetCement _cement;

        public bool CanPlace(RaycastHit hit, out GameObject targetToConsume)
        {
            targetToConsume = null;
            _cement = null;

            if (!_brickData.AllowedSurfaces.Contains(hit.transform.gameObject.layer))
            {
                return false;
            }

            _cement = hit.collider.GetComponentInParent<WetCement>();
            if (_cement == null) return false;

            targetToConsume = _cement.gameObject;
            return true;
        }

        public void UpdatePreview(GameObject itemPreview, RaycastHit hit, bool canBuild)
        {
            if (_cement != null)
            {
                float cementTopY = _cement.GetComponentInChildren<Collider>().bounds.min.y;
                itemPreview.transform.position = new Vector3(hit.point.x, cementTopY, hit.point.z);
            }
            else
            {
                itemPreview.transform.position = hit.point;
            }
        }

        public void Build(Vector3 position, Quaternion rotation)
        {
            Instantiate(_brickData.BuildPrefab, position, rotation);
        }
    }
}