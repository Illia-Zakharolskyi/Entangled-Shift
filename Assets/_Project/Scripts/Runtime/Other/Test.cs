using System.Collections;
using UnityEngine;

public class BaseManager : MonoBehaviour
{
    [Header("")]
    [SerializeField] private GameObject mainBase;     

    [System.Serializable]
    public struct MazePair
    {
        public GameObject triggerCube;  
        public Transform mazeEntrance;  
    }

    [Header("")]
    [SerializeField] private MazePair[] mazePairs;

    [Header("")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("")]
    [SerializeField] private bool lockY = true; 
    [SerializeField] private bool lockZ = true; 

    private bool isMoving = false;

    private void Start()
    {
        foreach (var pair in mazePairs)
        {
            if (pair.triggerCube != null)
            {
                BaseTriggerListener listener = pair.triggerCube.GetComponent<BaseTriggerListener>();
                if (listener == null)
                {
                    listener = pair.triggerCube.AddComponent<BaseTriggerListener>();
                }

                listener.OnPlayerEntered += () => StartMovingBase(pair.mazeEntrance);
            }
        }
    }

    private void StartMovingBase(Transform targetEntrance)
    {
        if (mainBase != null && targetEntrance != null && !isMoving)
        {
            StartCoroutine(MoveBaseRoutine(targetEntrance));
        }
    }

    private IEnumerator MoveBaseRoutine(Transform targetEntrance)
    {
        isMoving = true;

        Vector3 targetPosition = new Vector3(
            targetEntrance.position.x,
            lockY ? mainBase.transform.position.y : targetEntrance.position.y,
            lockZ ? mainBase.transform.position.z : targetEntrance.position.z
        );

        while (Vector3.Distance(mainBase.transform.position, targetPosition) > 0.01f)
        {
            mainBase.transform.position = Vector3.MoveTowards(
                mainBase.transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );

            mainBase.transform.rotation = Quaternion.RotateTowards(
                mainBase.transform.rotation,
                targetEntrance.rotation,
                moveSpeed * 50f * Time.deltaTime
            );

            yield return null;
        }

        mainBase.transform.position = targetPosition;
        mainBase.transform.rotation = targetEntrance.rotation;

        isMoving = false;
        Debug.Log("База успешно сместилась параллельно!");
    }
}

public class BaseTriggerListener : MonoBehaviour
{
    public System.Action OnPlayerEntered;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnPlayerEntered?.Invoke();
        }
    }
}