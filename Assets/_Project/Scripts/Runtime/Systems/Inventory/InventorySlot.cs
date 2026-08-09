using System;

[Serializable]
public class InventorySlot
{
    public ItemData item;
    public int amount;
    public int index;

    public bool IsEmpty => item == null || amount <= 0;

    public InventorySlot(ItemData item, int amount, int index)
    {
        this.item = item;
        this.amount = amount;
        this.index = index;
    }

    public void Clear()
    {
        item = null;
        amount = 0;
    }
}
