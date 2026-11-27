using EntangledShift.Runtime.Systems;
using UnityEngine;

namespace EntangledShift.Systems.Building
{
    public class BuildSystem : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private InventoryData _inventoryData;
        [SerializeField] private InventoryEvents _inventoryEvents;
        [SerializeField] private Camera _mainCamera;
        [SerializeField] private AudioClip _clip;

        [Header("Settings")]
        [SerializeField] private float BuildRange;
        public bool BuildingEnabled { get; set; } = true;

        private BuildState _state;

        private BuildableItemData _itemData;
        private GameObject _itemPreview;
        private IPlacementHandler _itemValidator;

        private void Update()
        {
            UpdateStateMachine();
        }

        private void UpdateStateMachine()
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

        private void CheckActiveSlot()
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

        private void ResetItem()
        {
            if (_itemPreview != null)
            {
                Destroy(_itemPreview);
            }

            _itemData = null;
            _itemPreview = null;
            _itemValidator = null;
        }

        private void HandleActiveBuilding()
        {
            if (_itemValidator == null)
            {
                return;
            }

            Vector3 screenCenter = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f);
            Ray ray = _mainCamera.ScreenPointToRay(screenCenter);

            _itemPreview.SetActive(true);

            bool canPlace = false;
            RaycastHit hit;

            bool hasHit = _itemValidator.Raycast(ray, out hit, BuildRange);

            if (!hasHit)
            {
                hit = new RaycastHit();
                hit.point = ray.GetPoint(BuildRange);
            }

            // Position the preview first so any transform-based checks inside CanPlace use the current target spot
            UpdatePreview(hit, false);

            if (hasHit)
            {
                canPlace = _itemValidator.CanPlace(hit);
            }

            // Apply final placement state (position, socket snap, and valid/invalid color)
            UpdatePreview(hit, canPlace);

            if (canPlace && Input.GetMouseButtonDown(0))
            {
                ExecuteBuild();
            }
        }

        private void ExecuteBuild()
        {
            if (!BuildingEnabled)
            {
                return;
            }

            if (AudioController.Instance != null) AudioController.Instance.PlayOneShotSFXSound(_clip);
            _itemValidator.Build(_itemPreview.transform.position, _itemPreview.transform.rotation);
            _inventoryEvents.InvokeActiveSlotRemove(1);
        }

        private void UpdatePreview(RaycastHit hit, bool canPlace)
        {
            _itemValidator.UpdatePreview(_itemPreview, hit, canPlace);
        }
    }
}