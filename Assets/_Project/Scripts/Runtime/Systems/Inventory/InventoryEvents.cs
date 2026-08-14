using System;
using UnityEngine;

[CreateAssetMenu(fileName = "InventoryEvents", menuName = "Scriptable Objects/InventoryEvents")]
public class InventoryEvents : ScriptableObject
{
    #region General Events
    public event Action<ItemData, int> OnItemAdd;
    public event Action<ItemData, int> OnItemRemove;
    public event Action<int> OnItemRemoveIndex;
    public event Action OnOpen;
    public event Action OnClose;
    #endregion

    #region Slot Events
    public event Action<InventorySlotUI> OnSlotStartDrag;
    public event Action OnSlotDragging;
    public event Action<InventorySlotUI> OnSlotEndDrag;
    public event Action<InventorySlotUI, InventorySlotUI> OnSwapSlots;
    public event Action<InventorySlotUI> OnSlotTooltip;
    public event Action<InventorySlotUI> OnActiveSlotChange;
    #endregion

    #region General Methods
    public void InvokeItemAdd(ItemData data, int amount) =>OnItemAdd?.Invoke(data, amount);
    public void InvokeItemRemove(ItemData data, int amount) =>OnItemRemove?.Invoke(data, amount);
    public void InvokeItemRemoveIndex(int index) => OnItemRemoveIndex?.Invoke(index);
    public void InvokeOpen() => OnOpen?.Invoke();
    public void InvokeClose() => OnClose?.Invoke();
    #endregion

    #region Slot Methods
    public void InvokeSlotStartDrag(InventorySlotUI slot) => OnSlotStartDrag?.Invoke(slot);
    public void InvokeSlotDragging() => OnSlotDragging?.Invoke();
    public void InvokeSlotEndDrag(InventorySlotUI slot) => OnSlotEndDrag?.Invoke(slot);
    public void InvokeSwapSlots(InventorySlotUI slot1, InventorySlotUI slot2) => OnSwapSlots?.Invoke(slot1, slot2);
    public void InvokeSlotTooltip(InventorySlotUI data) => OnSlotTooltip?.Invoke(data);
    public void InvokeActiveSlotChange(InventorySlotUI slotUI) => OnActiveSlotChange?.Invoke(slotUI);
    #endregion
}
