using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class Inventory : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private InventoryEvents _events;
    [SerializeField] private InventoryData _data;
    [SerializeField] private UIEvents _eventsUI;

    [Header("Slot Parents")]
    [SerializeField] private Transform _mainSlotsParent;
    [SerializeField] private Transform _hotbarSlotsParent;

    [Header("Settings")]
    [SerializeField] private int _capacity = 36;

    [Header("Inventory Testing")]
    [SerializeField] private List<InventoryTesting> _items;

    private int CurrentSlotIndex;

    private InventorySlot[] _allSlots;
    private InventorySlotUI[] _allSlotsUI;
    private InventorySlotUI[] _mainSlotsUI;
    private InventorySlotUI[] _hotbarSlotsUI;

    private Actions _actions;
    private bool _isOpen = false;

    #region MonoBehaviour & Awake-Only Methods
    private void Awake()
    {
        _actions = new Actions();

        _allSlots = new InventorySlot[_capacity];
        _allSlotsUI = new InventorySlotUI[_capacity];

        InventorySlotUI[] mainSlotsUI = _mainSlotsParent.GetComponentsInChildren<InventorySlotUI>(true);
        InventorySlotUI[] hotbarSlotsUI = _hotbarSlotsParent.GetComponentsInChildren<InventorySlotUI>(true);
        _mainSlotsUI = new InventorySlotUI[mainSlotsUI.Length];
        _hotbarSlotsUI = new InventorySlotUI[hotbarSlotsUI.Length];

        int indexGeneral = 0;

        for (int localIndex = 0; localIndex < _hotbarSlotsUI.Length; localIndex++)
        {
            InitializeSlot(hotbarSlotsUI[localIndex], indexGeneral, _hotbarSlotsUI, localIndex);
            indexGeneral++;
        }

        for (int localIndex = 0; localIndex < mainSlotsUI.Length; localIndex++)
        {
            InitializeSlot(mainSlotsUI[localIndex], indexGeneral, _mainSlotsUI, localIndex);
            indexGeneral++;
        }

        foreach (var item in _items)
        {
            if (item.Item != null && item.Amount > 0)
            {
                AddItem(item.Item, item.Amount);
            }
        }
    }

    private void InitializeSlot(InventorySlotUI slotReal, int indexGeneral, InventorySlotUI[] slotsForInit, int indexLocal)
    {
        slotsForInit[indexLocal] = slotReal;
        slotsForInit[indexLocal].Clear();
        slotsForInit[indexLocal].Initialize(indexGeneral);

        _allSlotsUI[indexGeneral] = slotReal;
        _allSlots[indexGeneral] = new InventorySlot(null, 0, indexGeneral);
    }


    private void OnEnable()
    {
        _actions.Enable();
        _actions.UI.ScrollWheel.performed += OnScroll;
        _actions.Player.ActiveSlot.performed += OnActiveSlot;
        _actions.Player.OpenInventory.performed += OnOpen;
        _events.OnItemAdd += AddItem;
        _events.OnItemRemove += RemoveItem;
        _events.OnItemRemoveIndex += RemoveItemIndex;
        _events.OnSwapSlots += SwapSlots;
        _events.OnOpen += OnOpen;
        _events.OnClose += OnClose;
    }

    private void OnDisable()
    {
        _actions.Disable();
        _actions.UI.ScrollWheel.performed -= OnScroll;
        _actions.Player.ActiveSlot.performed -= OnActiveSlot;
        _actions.Player.OpenInventory.performed -= OnOpen;
        _events.OnItemAdd -= AddItem;
        _events.OnItemRemove -= RemoveItem;
        _events.OnItemRemoveIndex -= RemoveItemIndex;
        _events.OnSwapSlots -= SwapSlots;
        _events.OnOpen -= OnOpen;
        _events.OnClose -= OnClose;

    }
    #endregion

    private void OnOpen()
    {
        foreach (InventorySlotUI slot in _mainSlotsUI)
        {
            slot.Show();
        }

        UpdateAllSlots();
    }

    private void OnOpen(InputAction.CallbackContext context)
    {
        if (!_isOpen)
        {
            _eventsUI.InvokeInventoryOpen();
            _isOpen = true;
        }
        else
        {
            _eventsUI.InvokeInventoryClose();
            _isOpen = false;
        }
    }

    private void OnClose()
    {
        foreach (InventorySlotUI slot in _mainSlotsUI)
        {
            slot.Hide();
        }
    }

    private void AddItem(ItemData itemToAdd, int amount = 1)
    {
        if (itemToAdd.IsStackable)
        {
            foreach (InventorySlot slot in _allSlots)
            {
                if (slot.item == itemToAdd && slot.amount < itemToAdd.MaxStackSize)
                {
                    _allSlotsUI[slot.index].ChangeData(itemToAdd);

                    int spaceLeft = itemToAdd.MaxStackSize - slot.amount;
                    int toAdd = Mathf.Min(spaceLeft, amount);

                    slot.amount += toAdd;
                    amount -= toAdd;

                    if (amount <= 0)
                    {
                        UpdateSlot(slot.index);
                        return;
                    }
                }
            }
        }

        while (amount > 0)
        {
            InventorySlot emptySlot = _allSlots.FirstOrDefault(s => s.IsEmpty);
            if (emptySlot == null)
            {
                return;
            }

            int toAdd = itemToAdd.IsStackable ? Mathf.Min(itemToAdd.MaxStackSize, amount) : 1;
            emptySlot.item = itemToAdd;
            emptySlot.amount = toAdd;
            amount -= toAdd;
            UpdateSlot(emptySlot.index);
        }
    }

    private void RemoveItem(ItemData itemToRemove, int amount = 1)
    {
        foreach (InventorySlot slot in _allSlots)
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
                    _allSlotsUI[slot.index].ChangeData(null);
                    slot.amount = 0;
                }
                UpdateSlot(slot.index);
                if (amount <= 0)
                {
                    break;
                }
            }
        }
    }

    private void RemoveItemIndex(int index)
    {
        if (!_allSlots[index].IsEmpty)
        {
            _allSlots[index].Clear();
            UpdateSlot(index);
        }
    }

    private void SwapSlots(InventorySlotUI slot1, InventorySlotUI slot2)
    {
        int index1 = slot1.Index;
        int index2 = slot2.Index;

        if (_allSlots[slot1.Index].IsEmpty)
        {
            Debug.Log("S");
            _allSlots[slot1.Index].amount = _allSlots[slot2.Index].amount;
            _allSlots[slot1.Index].item = _allSlots[slot2.Index].item;
            _allSlotsUI[slot1.Index].ChangeData(_allSlots[slot1.Index].item);
            RemoveItemIndex(slot2.Index);

            UpdateSlot(index1);
            UpdateSlot(index2);
            return;
        }

        bool isAddable = IsSlotsAddable(slot1, slot2);

        if (isAddable)
        {
            if (_allSlots[slot1.Index].amount + _allSlots[slot2.Index].amount <= slot1.Data.MaxStackSize)
            {
                _allSlots[slot1.Index].amount += _allSlots[slot2.Index].amount;
                RemoveItemIndex(slot2.Index);
                UpdateSlot(index1);
                return;
            }
        }

        ItemData tempItem = _allSlots[index1].item;
        int tempAmount = _allSlots[index1].amount;

        _allSlots[index1].item = _allSlots[index2].item;
        _allSlots[index1].amount = _allSlots[index2].amount;

        _allSlots[index2].item = tempItem;
        _allSlots[index2].amount = tempAmount;

        UpdateSlot(index1);
        UpdateSlot(index2);
    }

    private bool IsSlotsAddable(InventorySlotUI slot1, InventorySlotUI slot2)
    {
        return slot1.Data != null && slot2.Data != null && slot1.Data.IsStackable && slot2.Data.IsStackable && slot1.Data.Id == slot2.Data.Id;
    }

    private void UpdateAllSlots()
    {
        int i = 0;
        foreach (InventorySlotUI slot in _allSlotsUI)
        {
            slot.UpdateUI(_allSlots[i]);
            i++;
        }
    }

    private void UpdateSlot(int index)
    {
        _allSlotsUI[index].UpdateUI(_allSlots[index]);

        if (index == CurrentSlotIndex)
        {
            _data.activeSlot = _allSlots[CurrentSlotIndex];
            _data.activeSlotUI = _hotbarSlotsUI[CurrentSlotIndex];
            _events.InvokeActiveSlotChange(_hotbarSlotsUI[CurrentSlotIndex]);
        }
    }

    private void OnScroll(InputAction.CallbackContext context)
    {
        Vector2 scrollValue = context.ReadValue<Vector2>();

        if (scrollValue.y != 0)
        {
            int direction = scrollValue.y > 0 ? -1 : 1;

            CurrentSlotIndex += direction;

            if (CurrentSlotIndex >= _hotbarSlotsUI.Length)
            {
                CurrentSlotIndex = 0;
            }

            else if (CurrentSlotIndex < 0)
            {
                CurrentSlotIndex = _hotbarSlotsUI.Length - 1;
            }

            _events.InvokeActiveSlotChange(_hotbarSlotsUI[CurrentSlotIndex]);
            _data.activeSlotUI = _hotbarSlotsUI[CurrentSlotIndex];
            _data.activeSlot = _allSlots[CurrentSlotIndex];
        }
    }

    private void OnActiveSlot(InputAction.CallbackContext context)
    {
        Debug.Log("Ah");
        string keyName = context.control.name;

        if (int.TryParse(keyName, out int digit))
        {
            int index = digit - 1;

            CurrentSlotIndex = index;
            _events.InvokeActiveSlotChange(_hotbarSlotsUI[index]);
            _data.activeSlot = _allSlots[index];
            _data.activeSlotUI = _hotbarSlotsUI[index];
        }
    }
}

[System.Serializable]
public struct InventoryTesting
{
    [SerializeField] private ItemData _item;
    [SerializeField] private int _amount;

    public ItemData Item => _item;
    public int Amount => _amount;
}