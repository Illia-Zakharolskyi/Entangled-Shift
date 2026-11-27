using EntangledShift.Runtime.Systems;
using EntangledShift.Systems.Building;
using UnityEngine;

public class Chest : MonoBehaviour, IMainInteractable, IItemContainer, IDestroyable
{
    [Header("References")]
    [SerializeField] private GameObject _chestPanel;
    [SerializeField] private Transform _slotsParent;
    [SerializeField] private UIEvents _events;
    [SerializeField] private InventoryEvents _inventEvents;
    [SerializeField] private InventoryData _data;
    [SerializeField] private AudioClip _audio;
    [field: SerializeField] public Transform _top { get; private set; }
    [SerializeField] private ChestEvents _chestEvents;

    [Header("Item Drop References")]
    [SerializeField] private ItemData _chestItemData;
    [SerializeField] private ItemEvents _itemEvents;

    [Header("Settings")]
    [SerializeField] private int _capacity = 15;
    public string Name { get; private set; }

    private InventorySlot[] _slots;
    private InventorySlotUI[] _slotsUI;
    private bool _isOpen = false;

    private void Awake()
    {
        Name = _chestItemData.ShowName;
        ChestData data = FindAnyObjectByType<ChestData>();
        if (data != null)
        {
            _chestPanel = data.ChestPanel;
            _slotsParent = data.slotsParent;
            _slotsUI = _slotsParent.GetComponentsInChildren<InventorySlotUI>(true);
        }

        _slots = new InventorySlot[_capacity];
        for (int i = 0; i < _capacity; i++)
        {
            _slots[i] = new InventorySlot(null, 0, i);
        }
    }

    #region IInteractable Implementation
    public string GetPrompt()
    {
        return _isOpen ? "'E' Close" : "'E' Open";
    }

    public void MainInteract()
    {
        _isOpen = !_isOpen;
        if (_chestPanel != null)
        {
            _chestPanel.SetActive(_isOpen);
        }

        if (_isOpen)
        {
            if (_events != null) _events.InvokeChestOpen();
            BindAndRefreshUI();
        }
        else
        {
            if (_events != null) _events.InvokeChestClose();
        }

        if (AudioController.Instance) AudioController.Instance.PlayOneShotSFXSound(_audio);
        _chestEvents.InvokeChestInteract(this);
    }
    #endregion

    public void Destroy()
    {
        foreach (InventorySlot slot in _slots)
        {
            if (slot.item == null) continue;
            GameObject obj = Instantiate(slot.item.PrefabToDrop, transform.position, Quaternion.identity, transform.parent);
            obj.AddComponent<WorldItem>();
            WorldItem i = obj.GetComponent<WorldItem>();
            i.Init(slot.item, slot.amount, _inventEvents);
            RemoveItemIndex(slot.index);
        }
        Destroy(gameObject);
    }

    private void BindAndRefreshUI()
    {
        if (_slotsUI == null) return;

        for (int i = 0; i < _slotsUI.Length && i < _capacity; i++)
        {
            _slotsUI[i].Initialize(i, this);
            _slotsUI[i].UpdateUI(_slots[i]);
        }
    }

    #region IItemContainer Implementation
    public InventorySlot GetSlot(int index)
    {
        return _slots[index];
    }

    public void UpdateSlot(int index)
    {
        if (_slotsUI != null && index >= 0 && index < _slotsUI.Length)
        {
            _slotsUI[index].UpdateUI(_slots[index]);
        }
    }

    public void RemoveItemIndex(int index)
    {
        if (!_slots[index].IsEmpty)
        {
            _slots[index].Clear();
            UpdateSlot(index);
        }
    }
    #endregion
}