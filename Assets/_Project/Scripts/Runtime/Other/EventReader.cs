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

    void OnChestOpen()
    {
        _data.isChestOpen = true;
        _look.OnCursorUnlock();
        _look.enabled = false;
        _controller.enabled = false;
        _manager.enabled = false;
        _builder.enabled = false;
    }

    void OnChestClose()
    {
        _data.isChestOpen = false;
        _look.enabled = true;
        _controller.enabled = true;
        _manager.enabled = true;
        _inventory.enabled = true;
        _builder.enabled = true;
        _look.OnCursorLock();
    }

    void OnPauseOpen()
    {
        if (_look.enabled == true)
        {
            _look.OnCursorUnlock();

            _look.enabled = false;
            _controller.enabled = false;
            _manager.enabled = false;
        }
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
