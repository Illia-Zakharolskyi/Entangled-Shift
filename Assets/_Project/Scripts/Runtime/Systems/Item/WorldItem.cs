using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class WorldItem : MonoBehaviour, IInteractable
{
    public ItemData Data { get; private set; }
    public int Amount { get; private set; }

    private ItemEvents _events;

    public void Init(ItemData data, int amount, ItemEvents events)
    {
        Data = data;
        Amount = amount;

        _events = events;
    }

    public void Interact()
    {
        if (_events != null)
        {
            _events.InvokeInteractablePick(this);
        }

        Destroy(this.gameObject);
    }
}

public interface IInteractable
{
    void Interact();
}