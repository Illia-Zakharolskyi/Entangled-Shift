using UnityEngine;

public class Chest : MonoBehaviour, IMainInteractable, ISecondInteractable, IItemContainer
{
    [Header("References")]
    [SerializeField] private GameObject _chestPanel;
    [SerializeField] private Transform _slotsParent;
    [SerializeField] private UIEvents _events;
    [SerializeField] private InventoryEvents _inventEvents;

    [Header("Item Drop References")]
    [SerializeField] private ItemData _chestItemData;
    [SerializeField] private ItemEvents _itemEvents;

    [Header("Settings")]
    [SerializeField] private int _capacity = 15;

    private InventorySlot[] _slots;
    private InventorySlotUI[] _slotsUI;
    private bool _isOpen = false;

    private void Awake()
    {
        ChestData data = FindFirstObjectByType<ChestData>();
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
        return _isOpen ? "'E' Close" : "'E' Open || 'F' Pick Up";
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
    }

    public void SecondInteract()
    {
        if (_isOpen)
        {
            _isOpen = false;
            if (_chestPanel != null) _chestPanel.SetActive(false);
            if (_events != null) _events.InvokeChestClose();
        }

        foreach (InventorySlot slot in _slots)
        {
            if (slot != null && !slot.IsEmpty)
            {
                Vector3 spawnPosition = transform.position + Vector3.up * 0.5f + Random.insideUnitSphere * 0.3f;

                if (slot.item != null)
                {
                    GameObject spawnedObject = Instantiate(slot.item.PrefabToDrop, spawnPosition, Quaternion.identity);

                    if (spawnedObject.TryGetComponent<WorldItem>(out WorldItem worldItem))
                    {
                        worldItem.Init(slot.item, slot.amount, _inventEvents);
                    }
                }

                slot.Clear();
            }
        }

        if (_inventEvents != null && _chestItemData != null)
        {
            _inventEvents.InvokeItemAdd(_chestItemData, 1);
        }

        Destroy(gameObject);
    }
    #endregion

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