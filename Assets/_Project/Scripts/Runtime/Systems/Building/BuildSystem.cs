using UnityEngine;

namespace EntangledShift.Systems.Building
{
    public class BuildSystem : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private InventoryData _inventoryData;
        [SerializeField] private InventoryEvents _inventoryEvents;
        [SerializeField] private Camera _mainCamera;

        [Header("Settings")]
        [SerializeField] private float _buildRange;

        private BuildState _state;

        private BuildableItemData _itemData;
        private GameObject _itemPreview;
        private IPlacementHandler _itemValidator;

        void Update()
        {
            UpdateStateMachine();
        }

        void UpdateStateMachine()
        {
            CheckActiveSlot();

            switch (_state)
            {
                case BuildState.Idle:
                    break;

                case BuildState.Active:
                    HandleActiveBuilding();
                    break;

                case BuildState.Exit:
                    ResetItem();
                    _state = BuildState.Idle;
                    break;
            }
        }

        void CheckActiveSlot()
        {
            ItemData activeSlotData = _inventoryData.activeSlot.item;

            if (activeSlotData is not BuildableItemData)
            {
                if (_state != BuildState.Idle)
                {
                    _state = BuildState.Exit;
                }
                return;
            }

            BuildableItemData itemData = activeSlotData as BuildableItemData;

            if (itemData != _itemData)
            {
                ResetItem();
                _itemData = itemData;

                _itemPreview = Instantiate(_itemData.PreviewPrefab);
                _itemValidator = _itemPreview.GetComponent<IPlacementHandler>();

                _state = BuildState.Active;
            }
        }

        void ResetItem()
        {
            if (_itemPreview != null) Destroy(_itemPreview);
            _itemData = null;
            _itemPreview = null;
            _itemValidator = null;
        }

        void HandleActiveBuilding()
        {
            if (_itemValidator == null) return;

            Ray ray = _mainCamera.ScreenPointToRay(new Vector2(Screen.width / 2f, Screen.height / 2f));

            _itemPreview.SetActive(true);

            bool canPlace = false;
            GameObject targetToConsume = null;
            RaycastHit hit;

            bool hasHit = _itemValidator.Raycast(ray, out hit, _buildRange);

            if (hasHit)
            {
                canPlace = _itemValidator.CanPlace(hit, out targetToConsume);
            }
            else
            {
                hit = new RaycastHit();
                hit.point = ray.GetPoint(_buildRange);
            }

            UpdatePreview(hit, canPlace);

            if (canPlace && Input.GetMouseButtonDown(0))
            {
                ExecuteBuild(targetToConsume);
            }
        }


        private void ExecuteBuild(GameObject targetToConsume)
        {
            if (targetToConsume != null)
            {
                Destroy(targetToConsume);
            }

            _itemValidator.Build(_itemPreview.transform.position, _itemPreview.transform.rotation);
            _inventoryEvents.InvokeActiveSlotRemove(1);
        }

        private void UpdatePreview(RaycastHit hit, bool canPlace)
        {
            _itemValidator.UpdatePreview(_itemPreview, hit, canPlace);
        }
    }
}
