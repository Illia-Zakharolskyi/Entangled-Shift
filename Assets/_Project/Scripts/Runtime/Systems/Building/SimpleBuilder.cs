using EntangledShift.Systems.Building;
using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleBuilder : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InventoryData _inventoryData;
    [SerializeField] private InventoryEvents _inventoryEvents;
    [SerializeField] private Camera _mainCamera;

    [Header("Settings")]
    [SerializeField] private float _buildRange = 6f;
    [SerializeField] private LayerMask _buildMask;

    private GameObject _currentPreview;
    private BuildableItemData _currentBuildableData;

    private bool _isCustomRotation = false;
    private Quaternion _customRotationValue;

    private bool _isWorking = true;
    private bool _canPlaceCurrentFrame = false;

    private WetCement _targetWetCement;

    private void Update()
    {
        CheckActiveSlot();

        if (_currentBuildableData != null && _currentPreview != null && _isWorking)
        {
            UpdatePreviewPositionAndRotation();
            HandleInput();
        }
    }

    public void ChangeWorking(bool value)
    {
        _isWorking = value;
    }

    private void CheckActiveSlot()
    {
        if (_inventoryData.activeSlot == null || _inventoryData.activeSlot.item == null)
        {
            ClearPreview();
            return;
        }

        BuildableItemData buildableData = _inventoryData.activeSlot.item as BuildableItemData;

        if (buildableData == null)
        {
            ClearPreview();
            return;
        }

        if (buildableData != _currentBuildableData)
        {
            ClearPreview();
            _currentBuildableData = buildableData;

            if (_currentBuildableData.PreviewPrefab != null)
            {
                _currentPreview = Instantiate(_currentBuildableData.PreviewPrefab);
            }

            _isCustomRotation = false;
        }
    }

    private void UpdatePreviewPositionAndRotation()
    {
        Ray ray = _mainCamera.ScreenPointToRay(new Vector2(Screen.width / 2f, Screen.height / 2f));
        _targetWetCement = null;

        if (Physics.Raycast(ray, out RaycastHit hit, _buildRange, _buildMask))
        {
            int hitLayer = 1 << hit.collider.gameObject.layer;
            bool isValidLayer = (_currentBuildableData.AllowedSurfaces.value & hitLayer) != 0;

            bool isValidCementCondition = true;

            if (_currentBuildableData is CementBuildingItemData cementData && cementData.RequiresWetCement)
            {
                if (hit.collider.GetComponentInParent<WetCement>() is { } wetCement)
                {
                    _targetWetCement = wetCement;
                    isValidCementCondition = true;
                }
                else
                {
                    isValidCementCondition = false;
                }
            }

            _canPlaceCurrentFrame = isValidLayer && isValidCementCondition;

            if (!_currentPreview.activeSelf) _currentPreview.SetActive(true);

            Vector3 targetPosition = CalculatePerfectPosition(hit);
            _currentPreview.transform.position = targetPosition;

            if (!_isCustomRotation)
            {
                Vector3 cameraEuler = _mainCamera.transform.eulerAngles;
                _currentPreview.transform.rotation = Quaternion.Euler(0, cameraEuler.y, 0);
            }
            else
            {
                _currentPreview.transform.rotation = _customRotationValue;
            }
        }
        else
        {
            _canPlaceCurrentFrame = false;
            if (_currentPreview.activeSelf) _currentPreview.SetActive(false);
        }
    }

    private Vector3 CalculatePerfectPosition(RaycastHit hit)
    {
        if (_targetWetCement != null)
        {
            if (_targetWetCement.TryGetComponent<Renderer>(out var cementRenderer))
            {
                float topY = cementRenderer.bounds.min.y;
                return new Vector3(hit.point.x, topY, hit.point.z);
            }
            return _targetWetCement.transform.position;
        }

        if (hit.collider.TryGetComponent<Renderer>(out var targetRenderer))
        {
            if (hit.normal.y > 0.5f)
            {
                float topY = targetRenderer.bounds.max.y;
                return new Vector3(hit.point.x, topY, hit.point.z);
            }
        }

        return hit.point;
    }

    private void HandleInput()
    {
        if (!_currentPreview.activeInHierarchy) return;

        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            if (!_isCustomRotation)
            {
                _isCustomRotation = true;
                _customRotationValue = _currentPreview.transform.rotation;
            }
            _customRotationValue *= Quaternion.Euler(0, 90f, 0);
        }

        if (Mouse.current.leftButton.wasPressedThisFrame && _canPlaceCurrentFrame)
        {
            Build();
        }
    }

    private void Build()
    {

        if (_targetWetCement != null)
        {
            Destroy(_targetWetCement.gameObject);
            _targetWetCement = null;
        }

        if (_currentBuildableData.BuildPrefab != null)
        {
            Instantiate(_currentBuildableData.BuildPrefab, _currentPreview.transform.position, _currentPreview.transform.rotation);
        }

        _inventoryEvents.InvokeActiveSlotRemove(1);
    }

    private void ClearPreview()
    {
        if (_currentPreview != null)
        {
            Destroy(_currentPreview);
            _currentPreview = null;
        }
        _currentBuildableData = null;
        _targetWetCement = null;
        _isCustomRotation = false;
        _canPlaceCurrentFrame = false;
    }
}