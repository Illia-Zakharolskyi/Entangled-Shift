using UnityEngine;

[CreateAssetMenu(fileName = "InventoryData", menuName = "Scriptable Objects/InventoryData")]
public class InventoryData : ScriptableObject
{
    public InventorySlot activeSlot;
    public InventorySlotUI activeSlotUI;
}
