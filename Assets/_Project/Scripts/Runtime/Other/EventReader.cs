using EntangledShift.Systems.Building;
using UnityEngine;

public class EventReader : MonoBehaviour
{
    [SerializeField] private InventoryEvents _inventoryEvents;
    [SerializeField] private UIEvents _events;
    [SerializeField] private PlayerMouseLook _look;
    [SerializeField] private EntangledShift.Runtime.Other.PlayerController _controller;
    [SerializeField] private EquipmentManager _manager;
    [SerializeField] private Inventory _inventory;
    [SerializeField] private GameUIData _data;
    [SerializeField] private BuildSystem _system;

    private void OnEnable()
    {
        _inventoryEvents.OnOpen += OnInventoryOpen;
        _inventoryEvents.OnClose += OnInventoryClose;
        _events.OnPause += OnPauseOpen;
        _events.OnUnPause += OnPauseClose;
        _events.OnChestOpen += OnChestOpen;
        _events.OnChestClose += OnChestClose;
    }

    private void OnDisable()
    {
        _inventoryEvents.OnOpen -= OnInventoryOpen;
        _inventoryEvents.OnClose -= OnInventoryClose;
        _events.OnPause -= OnPauseOpen;
        _events.OnUnPause -= OnPauseClose;
        _events.OnChestOpen -= OnChestOpen;
        _events.OnChestClose -= OnChestClose;
    }

    private void OnInventoryOpen()
    {
        if (_look != null)
        {
            _look.OnCursorUnlock();
        }

        if (_look != null)
        {
            _look.enabled = false;
        }

        if (_controller != null)
        {
            _controller.enabled = false;
        }

        if (_system != null)
        {
            _system.BuildingEnabled = false;
        }
    }

    private void OnInventoryClose()
    {
        if (_look != null)
        {
            _look.OnCursorLock();
        }

        if (_look != null)
        {
            _look.enabled = true;
        }

        if (_controller != null)
        {
            _controller.enabled = true;
        }

        if (_system != null)
        {
            _system.BuildingEnabled = true;
        }
    }

    private void OnChestOpen()
    {
        _data.isChestOpen = true;
        if (_look != null)
        {
            _look.OnCursorUnlock();
        }

        if (_look != null)
        {
            _look.enabled = false;
        }

        if (_controller != null)
        {
            _controller.enabled = false;
        }

        if (_manager != null)
        {
            _manager.OnWorkDeny();
        }

        if (_inventory != null)
        {
            _inventory.OnSlotSwitchingDeny();
        }

        if (_system != null)
        {
            _system.BuildingEnabled = false;
        }
    }

    private void OnChestClose()
    {
        _data.isChestOpen = false;
        if (_look != null)
        {
            _look.enabled = true;
        }

        if (_controller != null)
        {
            _controller.enabled = true;
        }

        if (_manager != null)
        {
            _manager.OnWorkAllowed();
        }

        if (_look != null)
        {
            _look.OnCursorLock();
        }

        if (_inventory != null)
        {
            _inventory.OnSlotSwitchingAllow();
        }

        if (_system != null)
        {
            _system.BuildingEnabled = true;
        }
    }

    private void OnPauseOpen()
    {
        if (_system != null)
        {
            _system.BuildingEnabled = false;
        }

        if (_look != null && _look.enabled == true)
        {
            _look.OnCursorUnlock();

            _look.enabled = false;
            if (_controller != null)
            {
                _controller.enabled = false;
            }

            _manager.enabled = false;
        }
    }

    private void OnPauseClose()
    {
        if (_look != null)
        {
            _look.enabled = true;
        }

        if (_controller != null)
        {
            _controller.enabled = true;
        }

        if (_manager != null)
        {
            _manager.enabled = true;
        }

        if (_inventory != null)
        {
            _inventory.enabled = true;
        }

        if (_system != null)
        {
            _system.BuildingEnabled = true;
        }

        if (_look != null)
        {
            _look.OnCursorLock();
        }
    }
}
