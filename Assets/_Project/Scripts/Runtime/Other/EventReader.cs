using System;
using UnityEngine;

public class EventReader : MonoBehaviour
{
    [SerializeField] private InventoryEvents _inventoryEvents;
    [SerializeField] private UIEvents _events;
    [SerializeField] private PlayerMouseLook _look;
    [SerializeField] private PlayerControllerA _controller;
    [SerializeField] private EquipmentManager _manager;
    [SerializeField] private Inventory _inventory;
    [SerializeField] private SimpleBuilder _builder;
    [SerializeField] private GameUIData _data;

    void OnEnable()
    {
        _inventoryEvents.OnOpen += OnInventoryOpen;
        _inventoryEvents.OnClose += OnInventoryClose;
        _events.OnPause += OnPauseOpen;
        _events.OnUnPause += OnPauseClose;
        _events.OnChestOpen += OnChestOpen;
        _events.OnChestClose += OnChestClose;
    }

    void OnDisable()
    {
        _inventoryEvents.OnOpen -= OnInventoryOpen;
        _inventoryEvents.OnClose -= OnInventoryClose;
        _events.OnPause -= OnPauseOpen;
        _events.OnUnPause -= OnPauseClose;
        _events.OnChestOpen -= OnChestOpen;
        _events.OnChestClose -= OnChestClose;
    }

    void OnInventoryOpen()
    {
        if (_look != null) _look.OnCursorUnlock();

        if (_look != null) _look.enabled = false;
        if (_controller != null) _controller.enabled = false;
    }

    void OnInventoryClose()
    {
        if (_look != null) _look.OnCursorLock();

        if (_look != null) _look.enabled = true;
        if (_controller != null) _controller.enabled = true;
    }

    void OnChestOpen()
    {
        _data.isChestOpen = true;
        if (_look != null) _look.OnCursorUnlock();
        if (_look != null) _look.enabled = false;
        if (_controller != null) _controller.enabled = false;
        if (_manager != null) _manager.OnWorkDeny();
        if (_builder != null) _builder.ChangeWorking(false);
        if (_inventory != null) _inventory.OnSlotSwitchingDeny();
    }

    void OnChestClose()
    {
        _data.isChestOpen = false;
        if (_look != null) _look.enabled = true;
        if (_controller != null) _controller.enabled = true;
        if (_manager != null) _manager.OnWorkAllowed();
        if (_builder != null) _builder.ChangeWorking(true);
        if (_look != null) _look.OnCursorLock();
        if (_inventory != null) _inventory.OnSlotSwitchingAllow();
    }

    void OnPauseOpen()
    {
        if (_look != null && _look.enabled == true)
        {
            _look.OnCursorUnlock();

            _look.enabled = false;
            if (_controller != null) _controller.enabled = false;
            _manager.enabled = false;
        }
    }

    void OnPauseClose()
    {
        if (_look != null) _look.enabled = true;
        if (_controller != null) _controller.enabled = true;
        if (_manager != null) _manager.enabled = true;
        if (_inventory != null) _inventory.enabled = true;
        if (_look != null) _look.OnCursorLock();
    }
}
