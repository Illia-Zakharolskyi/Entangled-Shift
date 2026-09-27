using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class WorldItem : MonoBehaviour, IMainInteractable
{
    public ItemData Data { get; private set; }
    public int Amount { get; private set; }

    [SerializeField] private InventoryEvents _events;

    public void Init(ItemData data, int amount, InventoryEvents events)
    {
        Data = data;
        Amount = amount;

        _events = events;
    }

    public string GetPrompt()
    {
        return "'E' Pick Up";
    }

    public void MainInteract()
    {
        if (_events != null)
        {
            _events.InvokeItemAdd(Data, Amount);
        }

        Destroy(this.gameObject);
    }
}

public interface IMainInteractable
{
    string GetPrompt();
    void MainInteract();
}

public interface ISecondInteractable
{
    void SecondInteract();
}

public interface IItemContainer
{
    InventorySlot GetSlot(int index);
    void UpdateSlot(int index);
    void RemoveItemIndex(int index);
}