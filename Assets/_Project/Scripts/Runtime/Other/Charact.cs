
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class CharacterAutoRunner : MonoBehaviour
{
    [Header("Ссылки")]
    [SerializeField] private NavMeshAgent agent;         
    [SerializeField] private GameObject[] targetCubes;   
    [Header("Настройки задержки (в секундах)")]
    [SerializeField] private float minDelay = 1f;         
    [SerializeField] private float maxDelay = 2f;         

    private int currentIndex = -1;                       

    private void Start()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (targetCubes != null && targetCubes.Length > 0)
        {
            
            StartCoroutine(AutoRunRoutine());
        }
        else
        {
            Debug.LogWarning("Массив кубов (Target Cubes) пустой!");
        }
    }

    private IEnumerator AutoRunRoutine()
    {
        while (true)
        {
           
            float waitTime = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(waitTime);

           
            int newIndex;
            if (targetCubes.Length > 1)
            {
                do
                {
                    newIndex = Random.Range(0, targetCubes.Length);
                }
                while (newIndex == currentIndex);
            }
            else
            {
                newIndex = 0;
            }

            currentIndex = newIndex;
            GameObject targetCube = targetCubes[currentIndex];

            if (targetCube != null)
            {
                
                agent.SetDestination(targetCube.transform.position);

               
                while (agent.pathPending || agent.remainingDistance > 1f)
                {
                    yield return null;
                }
            }
        }
    }
}
