using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [SerializeField] private int capacity = 20;
    [SerializeField] private InventoryEvents _events;
    [SerializeField] private Transform[] _slotParents;
    [SerializeField] private ItemData _item1;
    [SerializeField] private ItemData _item2;

    private InventorySlot[] _slots;
    private InventorySlotUI[] _slotsUI;

    #region MonoBehaviour Methods
    private void Awake()
    {
        _slots = new InventorySlot[capacity];
        _slotsUI = new InventorySlotUI[capacity];

        int i = 0;
        foreach (Transform parent in _slotParents)
        {
            InventorySlotUI[] currUiSlots = parent.GetComponentsInChildren<InventorySlotUI>(true);
            foreach (InventorySlotUI slotUi in currUiSlots)
            {
                _slots[i] = new InventorySlot(null, 0, i);
                _slotsUI[i] = slotUi;

                _slotsUI[i].Clear();
                _slotsUI[i].Initialize(i);
                i++;
            }
        }

        AddItem(_item1, 2);
        AddItem(_item2, 3);
    }

    private void OnEnable()
    {
        _events.OnItemAdd += AddItem;
        _events.OnItemRemove += RemoveItem;
        _events.OnSwapSlots += SwapSlots;
        _events.OnOpen += OnOpen;
        _events.OnClose += OnClose;
    }

    private void OnDisable()
    {
        _events.OnItemAdd -= AddItem;
        _events.OnItemRemove -= RemoveItem;
        _events.OnSwapSlots -= SwapSlots;
        _events.OnOpen -= OnOpen;
        _events.OnClose -= OnClose;

    }
    #endregion

    private void OnOpen()
    {
        foreach (InventorySlotUI slot in _slotsUI)
        {
            if (slot.Index <= 5) // hotbar
            {
                continue;
            }

            slot.Show();
        }

        UpdateAllSlots();
    }

    private void OnClose()
    {
        foreach (InventorySlotUI slot in _slotsUI)
        {
            if (slot.Index <= 5) // hotbar
            {
                continue;
            }

            slot.Hide();
        }
    }

    private void AddItem(ItemData itemToAdd, int amount = 1)
    {
        if (itemToAdd.IsStackable)
        {
            foreach (InventorySlot slot in _slots)
            {
                if (slot.item == itemToAdd && slot.amount < itemToAdd.MaxStackSize)
                {
                    _slotsUI[slot.index].ChangeData(itemToAdd);

                    int spaceLeft = itemToAdd.MaxStackSize - slot.amount;
                    int toAdd = Mathf.Min(spaceLeft, amount);

                    slot.amount += toAdd;
                    amount -= toAdd;

                    if (amount <= 0)
                    {
                        UpdateAllSlots();
                        return;
                    }
                }
            }
        }

        while (amount > 0)
        {
            InventorySlot emptySlot = _slots.FirstOrDefault(s => s.IsEmpty);
            if (emptySlot == null) return;

            int toAdd = itemToAdd.IsStackable ? Mathf.Min(itemToAdd.MaxStackSize, amount) : 1;
            emptySlot.item = itemToAdd;
            emptySlot.amount = toAdd;
            amount -= toAdd;
        }

        UpdateAllSlots();
    }

    private void RemoveItem(ItemData itemToRemove, int amount = 1)
    {
        foreach (InventorySlot slot in _slots)
        {
            if (slot.item == itemToRemove)
            {
                if (slot.amount > amount)
                {
                    slot.amount -= amount;
                    amount = 0;
                }
                else
                {
                    amount -= slot.amount;
                    slot.item = null;
                    _slotsUI[slot.index].ChangeData(null);
                    slot.amount = 0;
                }
                if (amount <= 0) break;
            }
        }
        UpdateAllSlots();
    }

    private void RemoveItemIndex(int index)
    {
        if (!_slots[index].IsEmpty)
        {
            _slots[index].Clear();
            UpdateAllSlots();
        }
    }

    private void SwapSlots(InventorySlotUI slot1, InventorySlotUI slot2)
    {
        int index1 = slot1.Index;
        int index2 = slot2.Index;

        if (_slots[slot1.Index].IsEmpty)
        {
            Debug.Log("S");
            _slots[slot1.Index].amount = _slots[slot2.Index].amount;
            _slots[slot1.Index].item = _slots[slot2.Index].item;
            _slotsUI[slot1.Index].ChangeData(_slots[slot1.Index].item);
            RemoveItemIndex(slot2.Index);

            UpdateAllSlots();
            return;
        }

        bool isAddable = IsSlotsAddable(slot1, slot2);

        if (isAddable)
        {
            if (_slots[slot1.Index].amount + _slots[slot2.Index].amount <= slot1.Data.MaxStackSize)
            {
                _slots[slot1.Index].amount += _slots[slot2.Index].amount;
                RemoveItemIndex(slot2.Index);
                UpdateAllSlots();
                return;
            }
        }

        ItemData tempItem = _slots[index1].item;
        int tempAmount = _slots[index1].amount;

        _slots[index1].item = _slots[index2].item;
        _slots[index1].amount = _slots[index2].amount;

        _slots[index2].item = tempItem;
        _slots[index2].amount = tempAmount;

        UpdateAllSlots();
    }

    private bool IsSlotsAddable(InventorySlotUI slot1, InventorySlotUI slot2)
    {
        return slot1.Data != null && slot2.Data != null && slot1.Data.IsStackable && slot2.Data.IsStackable && slot1.Data.Id == slot2.Data.Id;
    }

    private void UpdateAllSlots()
    {
        int i = 0;
        foreach (InventorySlotUI slot in _slotsUI)
        {
            slot.UpdateUI(_slots[i]);
            i++;
        }
    }
}
