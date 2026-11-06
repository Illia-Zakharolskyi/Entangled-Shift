using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(MeshCollider))]
public class MetalDeformationPhysics : MonoBehaviour
{
    public bool canDeform = true;
    public float deformationStrength = 0.3f;
    public float deformationRadius = 0.8f;
    public LayerMask groundLayer;
    
    private Mesh visualMesh;
    private Mesh physicsMesh;
    private Vector3[] originalVertices;
    private Vector3[] modifiedVertices;
    private MeshCollider meshCollider;
    private Rigidbody rb;

    void Start()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
        rb = GetComponent<Rigidbody>();

        rb.isKinematic = false;

        meshCollider.convex = true;

        visualMesh = Instantiate(meshFilter.sharedMesh);
        physicsMesh = Instantiate(meshFilter.sharedMesh);

        meshFilter.mesh = visualMesh;
        meshCollider.sharedMesh = physicsMesh;

        originalVertices = visualMesh.vertices;
        modifiedVertices = new Vector3[originalVertices.Length];
        System.Array.Copy(originalVertices, modifiedVertices, originalVertices.Length);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!canDeform) return;

        if (((1 << collision.gameObject.layer) & groundLayer) != 0 || collision.collider.CompareTag("Plane"))
        {
            float impactForce = collision.relativeVelocity.magnitude;

            if (impactForce > 1.0f)
            {
                Vector3 contactPoint = collision.contacts[0].point;
                Vector3 localHitPoint = transform.InverseTransformPoint(contactPoint);
                Vector3 localHitDir = transform.InverseTransformDirection(-collision.relativeVelocity.normalized);

                DeformMesh(localHitPoint, localHitDir, impactForce);
            }
        }
    }

    void DeformMesh(Vector3 hitPoint, Vector3 hitDir, float force)
    {
        bool meshChanged = false;
        float adjustedStrength = deformationStrength * Mathf.Clamp(force * 0.1f, 0.2f, 2.0f);

        for (int i = 0; i < modifiedVertices.Length; i++)
        {
            float distance = Vector3.Distance(modifiedVertices[i], hitPoint);

            if (distance < deformationRadius)
            {
                float falloff = 1.0f - (distance / deformationRadius);
                modifiedVertices[i] += hitDir * (adjustedStrength * falloff);
                meshChanged = true;
            }
        }

        if (meshChanged)
        {
            visualMesh.vertices = modifiedVertices;
            visualMesh.RecalculateNormals();
            visualMesh.RecalculateBounds();

            physicsMesh.vertices = modifiedVertices;
            physicsMesh.RecalculateNormals();
            physicsMesh.RecalculateBounds();

            meshCollider.sharedMesh = null;
            meshCollider.sharedMesh = physicsMesh;

            rb.WakeUp();
        }
    }
}