using UnityEngine;

public class EventReader : MonoBehaviour
{
    [SerializeField] private InventoryEvents _inventoryEvents;
    [SerializeField] private UIEvents _events;
    [SerializeField] private PlayerMouseLook _look;
    [SerializeField] private PlayerControllerA _controller;

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
        _look.enabled = false;
        _controller.enabled = false;
    }

    void OnInventoryClose()
    {
        _look.enabled = true;
        _controller.enabled = true;
    }

    void OnPauseOpen()
    {
        _look.enabled = false;
        _controller.enabled = false;
    }

    void OnPauseClose()
    {
        _look.enabled = true;
        _controller.enabled = true;
    }
}
