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

    private void Update()
    {
        CheckActiveSlot();

        if (_currentBuildableData != null && _currentPreview != null)
        {
            UpdatePreviewPositionAndRotation();
            HandleInput();
        }
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

        if (Physics.Raycast(ray, out RaycastHit hit, _buildRange, _buildMask))
        {
            if (!_currentPreview.activeSelf) _currentPreview.SetActive(true);

            _currentPreview.transform.position = hit.point;

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
            if (_currentPreview.activeSelf) _currentPreview.SetActive(false);
        }
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

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Build();
        }
    }

    private void Build()
    {
        if (_currentBuildableData.BuildPrefab != null)
        {
            Instantiate(_currentBuildableData.BuildPrefab, _currentPreview.transform.position, _currentPreview.transform.rotation);
        }

        _inventoryEvents.InvokeItemRemove(_currentBuildableData, 1);
    }

    private void ClearPreview()
    {
        if (_currentPreview != null)
        {
            Destroy(_currentPreview);
            _currentPreview = null;
        }
        _currentBuildableData = null;
        _isCustomRotation = false;
    }
}