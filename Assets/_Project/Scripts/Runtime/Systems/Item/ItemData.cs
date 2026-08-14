using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
public class ItemData : ScriptableObject
{
    [SerializeField] private string _id;
    [SerializeField] private string _showName;
    [SerializeField] private Sprite _icon;
    [SerializeField] private bool _isStackable;
    [SerializeField] private int _maxStackSize;
    [SerializeField] private string _description;
    [SerializeField] private GameObject _prefabToEquip;
    [SerializeField] private GameObject _prefabToDrop;

    public string Id => _id;
    public string ShowName => _showName;
    public Sprite Icon => _icon;
    public bool IsStackable => _isStackable;
    public int MaxStackSize => _maxStackSize;
    public string Description => _description;
    public GameObject PrefabToEquip => _prefabToEquip;
    public GameObject PrefabToDrop => _prefabToDrop;
}
