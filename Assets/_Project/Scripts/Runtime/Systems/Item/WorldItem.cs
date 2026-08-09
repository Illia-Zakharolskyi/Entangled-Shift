using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(BoxCollider))]
public class WorldItem : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemData _data;
    [SerializeField] private SpriteRenderer _icon;
    [SerializeField] private ItemEvents _events;
    [SerializeField] private LayerMask _mask;
    [SerializeField] private int _amount;
    [SerializeField] private float _respawnTime = 3f;

    public int Amount => _amount;
    public ItemData Data => _data;

    private void Awake()
    {
        _icon.sprite = _data.Icon;
        GetComponent<BoxCollider>().isTrigger = true;
        GetComponent<BoxCollider>().size = new Vector3(10, 10, 10);
    }

    public void Interact()
    {
        if (_icon.enabled)
        {
            _events.InvokeInteractablePick(this);
            StartCoroutine(RespawnRoutine());
        }
    }

    private IEnumerator RespawnRoutine()
    {
        _icon.enabled = false;
        yield return new WaitForSeconds(_respawnTime);
        _icon.enabled = true;
    }
}

public interface IInteractable
{
    void Interact();
}