using TMPro;
using UnityEngine;

public class InventoryTooltip : MonoBehaviour
{
    [SerializeField] private GameObject _inventoryPanel;
    [SerializeField] private GameObject _tooltipPanel;
    [SerializeField] private TMP_Text _name;
    [SerializeField] private TMP_Text _desc;
    [SerializeField] private TMP_Text _index;
    [SerializeField] private InventoryEvents _events;

    private InventorySlotUI _lastSlot;

    #region MonoBehaviour
    private void OnEnable()
    {
        _events.OnSlotTooltip += Handle;
        _events.OnClose += Close;
    }

    private void OnDisable()
    {
        _events.OnSlotTooltip -= Handle;
        _events.OnClose -= Close;
    }
    #endregion

    private void Handle(InventorySlotUI slot)
    {
        if (!_inventoryPanel.activeInHierarchy)
        {
            return;
        }
        if (slot == null || _name == null || _desc == null)
        {
            Debug.Log("Wow");
            return;
        }
        if (_lastSlot != null && _lastSlot.Index == slot.Index)
        {
            Close();
            _lastSlot = null;
            return;
        }

        _tooltipPanel.SetActive(true);
        _name.text = slot.Data.ShowName;
        _desc.text = slot.Data.Description;
        _index.text = (slot.Index + 1).ToString();

        _lastSlot = slot;
    }

    private void Close()
    {
        _tooltipPanel.SetActive(false);
    }
}
