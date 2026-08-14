using UnityEngine;
using UnityEngine.InputSystem;

public class EquipmentManager : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private InventoryEvents _events;
    [SerializeField] private Transform _pointToEquip;
    [SerializeField] private InventoryData _data;
    [SerializeField] private ItemEvents _itemEvents;

    private GameObject _activeFrame = null;
    private GameObject _currentEquippedInstance = null;
    private Actions _action;

    private void Awake()
    {
        _action = new Actions();
    }

    private void OnEnable()
    {
        _action.Enable();
        _action.Player.Throw.performed += OnThrow;
        _events.OnActiveSlotChange += OnActiveSlotChange;
    }

    private void OnDisable()
    {
        _action.Disable();
        _action.Player.Throw.performed -= OnThrow;
        _events.OnActiveSlotChange -= OnActiveSlotChange;
    }

    private void OnActiveSlotChange(InventorySlotUI newSlot)
    {
        ClearEquippedItem();

        if (_activeFrame != null)
        {
            _activeFrame.SetActive(false);
        }

        newSlot.ActiveFrame.SetActive(true);
        _activeFrame = newSlot.ActiveFrame;

        if (newSlot.Data == null || newSlot.Data.PrefabToEquip == null)
        {
            return;
        }

        _currentEquippedInstance = Instantiate(newSlot.Data.PrefabToEquip, _pointToEquip);

        _currentEquippedInstance.transform.localPosition = Vector3.zero;
        _currentEquippedInstance.transform.localRotation = Quaternion.identity;
        _currentEquippedInstance.transform.localScale = Vector3.one;
    }

    private void OnThrow(InputAction.CallbackContext context)
    {
        if (_data == null || _data.activeSlotUI == null || _data.activeSlotUI.Data == null)
        {
            return;
        }

        ItemData currentData = _data.activeSlotUI.Data;

        GameObject worldItem = Instantiate(currentData.PrefabToDrop != null ? currentData.PrefabToDrop : currentData.PrefabToEquip,
                                           _pointToEquip.position,
                                           _pointToEquip.rotation);
        worldItem.GetComponent<BoxCollider>().enabled = true;

        if (!worldItem.TryGetComponent<WorldItem>(out var itemComponent))
        {
            itemComponent = worldItem.AddComponent<WorldItem>();
        }

        itemComponent.Init(currentData, _data.activeSlot.amount, _itemEvents);

        if (worldItem.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.AddForce(_pointToEquip.forward * 5f, ForceMode.Impulse);
        }

        _events.InvokeItemRemoveIndex(_data.activeSlot.index);
        ClearEquippedItem();
    }

    private void ClearEquippedItem()
    {
        if (_currentEquippedInstance != null)
        {
            Destroy(_currentEquippedInstance);
            _currentEquippedInstance = null;
        }
    }
}