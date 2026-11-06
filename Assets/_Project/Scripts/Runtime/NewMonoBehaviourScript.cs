using UnityEngine;
using UnityEngine.InputSystem;

public class SphereShooter : MonoBehaviour
{
    public GameObject spherePrefab;
    public Transform shootPoint;
    public float shootForce = 20f;

    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Shoot();
        }
    }

    void Shoot()
    {
        GameObject sphere = Instantiate(
            spherePrefab,
            shootPoint.position,
            shootPoint.rotation
        );

        Rigidbody rb = sphere.GetComponent<Rigidbody>();

        rb.AddForce(
            shootPoint.forward * shootForce,
            ForceMode.Impulse
        );
    }
}