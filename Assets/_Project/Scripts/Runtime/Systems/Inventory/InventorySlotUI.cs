using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerClickHandler
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private InventoryEvents _events;
    [SerializeField] private Image[] _frames;
    [SerializeField] private GameObject _activeFrame;

    private int _index;
    private ItemData _data;

    public Image Icon => icon;
    public int Index => _index;
    public ItemData Data => _data;
    public GameObject ActiveFrame => _activeFrame;
    private IItemContainer _container;
    public IItemContainer Container => _container;

    public void Initialize(int index, IItemContainer container)
    {
        _index = index;
        _index = index;
        _container = container;
    }

    public void ChangeData(ItemData data)
    {
        _data = data;
    }

    public void UpdateUI(InventorySlot slot)
    {
        if (slot == null || slot.IsEmpty)
        {
            Clear();
        }

        if (slot.IsEmpty)
        {
            icon.sprite = null;
            amountText.enabled = false;
            icon.color = new Color(255, 255, 255, 0);
        }
        else
        {
            icon.sprite = slot.item.Icon;
            icon.color = new Color(255, 255, 255, 255);
            _data = slot.item;

            if (slot.item.IsStackable && slot.amount > 1)
            {
                amountText.text = slot.amount.ToString();
                amountText.enabled = true;
            }
            else
            {
                amountText.enabled = false;
            }
        }
    }

    public void Clear()
    {
        _data = null;
        icon.sprite = null;
        amountText.enabled = false;
        icon.color = new Color(255, 255, 255, 0);
    }

    public void Show()
    {
        foreach (Image frame in _frames)
        {
            frame.enabled = true;
        }
        icon.color = _data != null ? new Color(255, 255, 255, 255) : new Color(255, 255, 255, 0);
    }

    public void Hide()
    {
        foreach (Image frame in _frames)
        {
            frame.enabled = false;
        }
        icon.color = new Color(255, 255, 255, 0);
    }

    #region Drag & Drop, Click
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right && _data != null)
        {
            _events.InvokeSlotTooltip(this);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _events.InvokeSlotStartDrag(this);
    }

    public void OnDrag(PointerEventData eventData)
    {
        _events.InvokeSlotDragging();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _events.InvokeSlotEndDrag(this);
    }

    public void OnDrop(PointerEventData eventData)
    {
        InventorySlotUI draggedSlot = eventData.pointerDrag?.GetComponent<InventorySlotUI>();

        if (draggedSlot == null)
        {
            return;
        }

        if (draggedSlot == this)
        {
            return;
        }

        _events.InvokeSwapSlots(draggedSlot, this);
    }
    #endregion
}