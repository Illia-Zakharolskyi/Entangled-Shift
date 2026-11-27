using TMPro;
using UnityEngine;

public class ItemInteractor : MonoBehaviour
{
    [SerializeField] private float _distance = 5f;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private LayerMask _mask;

    [Header("UI Hints")]
    [SerializeField] private GameObject _interactHintObject;
    [SerializeField] private TextMeshProUGUI _interactHintText;

    private void Update()
    {
        Ray ray = _mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        if (Physics.Raycast(ray, out RaycastHit hit, _distance, _mask))
        {
            IMainInteractable mainInteractable = hit.transform.root.GetComponentInChildren<IMainInteractable>();
            if (mainInteractable != null)
            {
                _interactHintObject.SetActive(true);
                if (_interactHintText != null)
                {
                    _interactHintText.text = mainInteractable.GetPrompt();
                }

                if (Input.GetKeyDown(KeyCode.E))
                {
                    mainInteractable.MainInteract();
                }
            }
            else
            {
                _interactHintObject.SetActive(false);
            }
        }
        else
        {
            _interactHintObject.SetActive(false);
        }
    }
}