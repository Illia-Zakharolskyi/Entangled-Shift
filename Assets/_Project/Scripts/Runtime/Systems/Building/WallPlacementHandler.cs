using UnityEngine;

namespace EntangledShift.Systems.Building
{
    public class WallPlacementHandler : MonoBehaviour, IPlacementHandler
    {
        [Header("References")]
        [SerializeField] private BuildableItemData _wallData;

        private BuildingSocket _targetSocket;

        public bool CanPlace(RaycastHit hit, out GameObject targetToConsume)
        {
            targetToConsume = null;
            _targetSocket = null;

            BuildingSocket socket = hit.collider.GetComponent<BuildingSocket>();
            if (socket == null || socket.IsOccupied)
            {
                return false;
            }

            _targetSocket = socket;
            return true;
        }

        public void UpdatePreview(GameObject itemPreview, RaycastHit hit, bool canPlace)
        {
            if (_targetSocket != null)
            {
                // Жестко привязываем позицию и поворот к сокету
                itemPreview.transform.position = _targetSocket.transform.position;
                itemPreview.transform.rotation = _targetSocket.transform.rotation;

                // Принудительно сбрасываем масштаб превью на единицы, 
                // чтобы игра случайно не растягивала стену по Y или другим осям
                itemPreview.transform.localScale = Vector3.one;
            }
            else
            {
                itemPreview.transform.position = hit.point;
            }
        }

        public void Build(Vector3 position, Quaternion rotation)
        {
            if (_targetSocket != null)
            {
                _targetSocket.Occupy();
            }

            GameObject newWall = Instantiate(_wallData.BuildPrefab, position, rotation);
            newWall.transform.localScale = Vector3.one;
        }
    }
}