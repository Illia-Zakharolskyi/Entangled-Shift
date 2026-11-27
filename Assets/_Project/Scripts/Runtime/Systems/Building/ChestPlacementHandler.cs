
using UnityEngine;

namespace EntangledShift.Systems.Building
{

    public class ChestPlacementHandler : PlacementHandler
    {
        public override bool CanPlace(RaycastHit hit)
        {
            //'Todo': idk, add logic but i don't see a reason for that(Layers already doing their jobs for this one)
            _targetSocket = null;
            return true;
        }

        public override void UpdatePreview(GameObject itemPreview, RaycastHit hit, bool canPlace)
        {
            UpdatePreviewColor(canPlace);
            itemPreview.transform.position = hit.point;
            /*if (hit.transform != null)
            {
                itemPreview.transform.position = hit.transform.position;
            }
            else
            {
                itemPreview.transform.position = hit.point;
            }*/
        }

        public override void Build(Vector3 position, Quaternion rotation)
        {
            GameObject obj = Instantiate(_data.BuildPrefab, position, rotation);
            Animate(obj, 0.5f, 0.25f);
        }
    }
}