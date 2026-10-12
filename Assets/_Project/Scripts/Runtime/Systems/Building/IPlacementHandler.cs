using UnityEngine;

namespace EntangledShift.Systems.Building
{
    public interface IPlacementHandler
    {
        bool CanPlace(RaycastHit hit, out GameObject targetToConsume);
        void UpdatePreview(GameObject _itemPreview, RaycastHit hit, bool canPlace);
        void Build(Vector3 position, Quaternion rotation);
    }
}
