using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryDragAndDrop : MonoBehaviour
{
    [SerializeField] private InventoryEvents _events;
    [SerializeField] private Image dragIcon;
    [SerializeField] private Canvas _canvas;
    [SerializeField] private Transform _parent;

    private InventorySlotUI _currSlot;
    private Actions.UIActions _action;
    

    #region MonoBehaviour Methods
    private void Awake()
    {
        Actions a = new Actions();
        a.Enable();
        _action = a.UI;
    }

    private void OnEnable()
    {
        _events.OnSlotStartDrag += HandleStartDrag;
        _events.OnSlotDragging += HandleDragging;
        _events.OnSlotEndDrag += HandleEndDrag;
    }

    private void OnDisable()
    {
        _action.Disable();
        _events.OnSlotStartDrag -= HandleStartDrag;
        _events.OnSlotDragging -= HandleDragging;
        _events.OnSlotEndDrag -= HandleEndDrag;

    }
    #endregion
   
    private void HandleStartDrag(InventorySlotUI slot)
    {
        if (!HasItemInSlot(slot)) return;
        _currSlot = slot;

        dragIcon.transform.SetParent(_canvas.transform);
        dragIcon.transform.SetAsLastSibling();

        dragIcon.sprite = GetSlotSprite(slot);
        dragIcon.raycastTarget = false;
        dragIcon.gameObject.SetActive(true);
        dragIcon.transform.position = Mouse.current.position.ReadValue();
    }

    private void HandleDragging()
    {
        if (_currSlot == null) return;
        dragIcon.transform.position = _action.Point.ReadValue<Vector2>();
    }

    private void HandleEndDrag(InventorySlotUI slot)
    {
        _currSlot = null;
        dragIcon.gameObject.SetActive(false);
        dragIcon.sprite = null;
        dragIcon.transform.SetParent(_parent);
    }

    private bool HasItemInSlot(InventorySlotUI slot)
    {
        return slot.Icon.enabled && slot.Icon.sprite != null;
    }
    private Sprite GetSlotSprite(InventorySlotUI slot)
    {
        return slot.Icon.sprite;
    }
}
