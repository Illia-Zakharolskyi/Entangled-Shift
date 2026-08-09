using UnityEngine;
using static UnityEditor.Progress;

public class ItemPicker : MonoBehaviour
{
    [SerializeField] private InventoryEvents _inventoryEvents;
    [SerializeField] private ItemEvents _events;
    [SerializeField] private int _distance;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private LayerMask _mask;
    [SerializeField] private GameObject _interactHint;

    private void OnEnable()
    {
        _events.OnInteractablePick += OnInteractable;
    }

    private void OnDisable()
    {
        _events.OnInteractablePick -= OnInteractable;
    }

    private void Update()
    {
        Ray ray = _mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        if (Physics.Raycast(ray, out RaycastHit hit, _distance, _mask))
        {
            if (hit.collider.TryGetComponent<IInteractable>(out IInteractable interactable))
            {
                _interactHint.SetActive(true);
                if (Input.GetKeyDown(KeyCode.E))
                {
                    interactable.Interact();
                    _interactHint.SetActive(false);
                }
            }
        }
        else
        {
            _interactHint.SetActive(false);
        }
    }

    private void OnInteractable(WorldItem item)
    {
        _inventoryEvents.InvokeItemAdd(item.Data, item.Amount);
    }
}
