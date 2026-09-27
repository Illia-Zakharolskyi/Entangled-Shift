using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class Inventory : MonoBehaviour, IItemContainer
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
        slotsForInit[indexLocal].Initialize(indexGeneral, this);

        _allSlotsUI[indexGeneral] = slotReal;
        _allSlots[indexGeneral] = new InventorySlot(null, 0, indexGeneral);
    }

    public InventorySlot GetSlot(int index)
    {
        return _allSlots[index];
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
        if (itemToAdd == null || amount <= 0) return;

        if (itemToAdd.IsStackable)
        {
            foreach (InventorySlot slot in _allSlots)
            {
                if (slot.item == itemToAdd && slot.amount < itemToAdd.MaxStackSize)
                {
                    int spaceLeft = itemToAdd.MaxStackSize - slot.amount;
                    int toAdd = Mathf.Min(spaceLeft, amount);

                    slot.amount += toAdd;
                    amount -= toAdd;

                    _allSlotsUI[slot.index].ChangeData(itemToAdd);
                    UpdateSlot(slot.index);

                    if (amount <= 0)
                    {
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
                Debug.LogWarning("[Inventory] Инвентарь полон!");
                return;
            }

            int toAdd = itemToAdd.IsStackable ? Mathf.Min(itemToAdd.MaxStackSize, amount) : 1;
            emptySlot.item = itemToAdd;
            emptySlot.amount = toAdd;
            amount -= toAdd;

            _allSlotsUI[emptySlot.index].ChangeData(itemToAdd);
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

    public void RemoveItemIndex(int index)
    {
        if (!_allSlots[index].IsEmpty)
        {
            _allSlots[index].Clear();
            UpdateSlot(index);
        }
    }

    private void SwapSlots(InventorySlotUI slot1, InventorySlotUI slot2)
    {
        IItemContainer container1 = slot1.Container;
        IItemContainer container2 = slot2.Container;

        InventorySlot data1 = container1.GetSlot(slot1.Index);
        InventorySlot data2 = container2.GetSlot(slot2.Index);

        if (data1.IsEmpty) return;

        if (data2.IsEmpty)
        {
            data2.amount = data1.amount;
            data2.item = data1.item;

            container1.RemoveItemIndex(slot1.Index);

            container1.UpdateSlot(slot1.Index);
            container2.UpdateSlot(slot2.Index);
            return;
        }

        bool isAddable = IsSlotsAddable(slot1, slot2);
        if (isAddable)
        {
            if (data1.amount + data2.amount <= slot1.Data.MaxStackSize)
            {
                data2.amount += data1.amount;
                container1.RemoveItemIndex(slot1.Index);

                container1.UpdateSlot(slot1.Index);
                container2.UpdateSlot(slot2.Index);
                return;
            }
            else
            {
                int spaceLeft = slot1.Data.MaxStackSize - data2.amount;
                data2.amount += spaceLeft;
                data1.amount -= spaceLeft;

                container1.UpdateSlot(slot1.Index);
                container2.UpdateSlot(slot2.Index);
                return;
            }
        }

        ItemData tempItem = data1.item;
        int tempAmount = data1.amount;

        data1.item = data2.item;
        data1.amount = data2.amount;

        data2.item = tempItem;
        data2.amount = tempAmount;

        container1.UpdateSlot(slot1.Index);
        container2.UpdateSlot(slot2.Index);
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

    public void UpdateSlot(int index)
    {
        if (index < 0 || index >= _allSlots.Length) return;

        _allSlotsUI[index].ChangeData(_allSlots[index].item);
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