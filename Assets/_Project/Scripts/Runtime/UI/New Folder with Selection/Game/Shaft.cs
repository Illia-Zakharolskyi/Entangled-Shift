using UnityEngine;

public class ShaftSection : MonoBehaviour
{
    [SerializeField] private float speed = 100f;
    [SerializeField] private float sectionHeight = 40f;
    [SerializeField] private float resetY = -40f;
    [SerializeField] private float topY = 120f;

    void Update()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime, Space.World);

        if (transform.position.y < resetY)
        {
            transform.position = new Vector3(
                transform.position.x,
                topY,
                transform.position.z);
        }
    }
}