using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
public class ItemData : ScriptableObject
{
    [SerializeField] private string id;
    [SerializeField] private string showName;
    [SerializeField] private Sprite icon;
    [SerializeField] private bool isStackable;
    [SerializeField] private int maxStackSize;
    [SerializeField] private string description;

    public string Id => id;
    public string ShowName => showName;
    public Sprite Icon => icon;
    public bool IsStackable => isStackable;
    public int MaxStackSize => maxStackSize;
    public string Description => description;
}
