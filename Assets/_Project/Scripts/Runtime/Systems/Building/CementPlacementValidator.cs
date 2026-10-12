using System;
using UnityEngine;

namespace EntangledShift.Systems.Building
{
    public class CementPlacementHandler : MonoBehaviour, IPlacementHandler
    {
        [Header("Refrences")]
        [SerializeField] private BuildableItemData _cementData;

        public bool CanPlace(RaycastHit hit, out GameObject objectConsume)
        {
            objectConsume = null;

            if (!_cementData.AllowedSurfaces.Contains(hit.transform.gameObject.layer))
            {
                return false;
            }

            return true;
        }

        public void Build(Vector3 position, Quaternion rotation)
        {
            Instantiate(_cementData.BuildPrefab, position, rotation);
        }

        public void UpdatePreview(GameObject itemPreview, RaycastHit hit, bool canPlace)
        {
            itemPreview.transform.position = hit.point;
        }
    }
}