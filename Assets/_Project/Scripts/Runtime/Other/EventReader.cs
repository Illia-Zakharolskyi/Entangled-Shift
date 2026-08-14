using UnityEngine;

public class EventReader : MonoBehaviour
{
    [SerializeField] private InventoryEvents _inventoryEvents;
    [SerializeField] private UIEvents _events;
    [SerializeField] private PlayerMouseLook _look;
    [SerializeField] private PlayerControllerA _controller;
    [SerializeField] private EquipmentManager _manager;
    [SerializeField] private Inventory _inventory;

    void OnEnable()
    {
        _inventoryEvents.OnOpen += OnInventoryOpen;
        _inventoryEvents.OnClose += OnInventoryClose;
        _events.OnPause += OnPauseOpen;
        _events.OnUnPause += OnPauseClose;
    }

    void OnDisable()
    {
        _inventoryEvents.OnOpen -= OnInventoryOpen;
        _inventoryEvents.OnClose -= OnInventoryClose;
        _events.OnPause -= OnPauseOpen;
        _events.OnUnPause -= OnPauseClose;
    }

    void OnInventoryOpen()
    {
        _look.OnCursorUnlock();

        _look.enabled = false;
        _controller.enabled = false;
    }

    void OnInventoryClose()
    {
        _look.OnCursorLock();

        _look.enabled = true;
        _controller.enabled = true;
    }

    void OnPauseOpen()
    {
        _look.OnCursorUnlock();

        _look.enabled = false;
        _controller.enabled = false;
        _manager.enabled = false;
        _inventory.enabled = false;
    }

    void OnPauseClose()
    {
        _look.enabled = true;
        _controller.enabled = true;
        _manager.enabled = true;
        _inventory.enabled = true;
        _look.OnCursorLock();
    }
}
