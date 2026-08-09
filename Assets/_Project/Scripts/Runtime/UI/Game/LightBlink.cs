using System.Collections;
using UnityEngine;

public class LightBlink : MonoBehaviour
{
    [SerializeField] private Light[] lights;

    [SerializeField] private float minWait = 3f;
    [SerializeField] private float maxWait = 8f;

    [SerializeField] private int minBlink = 2;
    [SerializeField] private int maxBlink = 5;

    [SerializeField] private float minBlinkTime = 0.03f;
    [SerializeField] private float maxBlinkTime = 0.08f;

    [SerializeField] private float minOff = 0.2f;
    [SerializeField] private float maxOff = 0.8f;

    private void Start()
    {
        StartCoroutine(Blink());
    }

    private IEnumerator Blink()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minWait, maxWait));

            int count = Random.Range(minBlink, maxBlink + 1);

            for (int i = 0; i < count; i++)
            {
                Set(false);
                yield return new WaitForSeconds(Random.Range(minBlinkTime, maxBlinkTime));

                Set(true);
                yield return new WaitForSeconds(Random.Range(minBlinkTime, maxBlinkTime));
            }

            Set(false);
            yield return new WaitForSeconds(Random.Range(minOff, maxOff));
            Set(true);
        }
    }

    private void Set(bool state)
    {
        foreach (Light light in lights)
        {
            if (light != null)
                light.enabled = state;
        }
    }
}