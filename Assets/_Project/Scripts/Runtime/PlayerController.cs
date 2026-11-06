
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 7f;
    public float jumpForce = 7f;
    public Rigidbody bodyRigidbody;
    public float rotationSpeed = 15f;

    public float raycastDistance = 1.1f;
    public float raycastOffset = 0.1f;

    public Transform playerCamera;

    private bool isGrounded;
    private bool canJump = true;

    void Start()
    {
        if (bodyRigidbody == null)
            bodyRigidbody = GetComponent<Rigidbody>();

        bodyRigidbody.constraints =
            RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationZ;
    }

    void Update()
    {
        CheckGround();

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && canJump)
        {
            Jump();
        }
    }

    void FixedUpdate()
    {
        if (playerCamera == null)
            return;

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 camForward = playerCamera.forward;
        Vector3 camRight = playerCamera.right;

        camForward.y = 0f;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir =
            (camForward * moveZ + camRight * moveX).normalized;

        Vector3 targetVelocity = moveDir * moveSpeed;

        bodyRigidbody.linearVelocity = new Vector3(
            targetVelocity.x,
            bodyRigidbody.linearVelocity.y,
            targetVelocity.z
        );

        if (camForward.sqrMagnitude > 0.001f)
        {
            if (moveDir.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDir);

                bodyRigidbody.MoveRotation(
                    Quaternion.RotateTowards(
                        bodyRigidbody.rotation,
                        targetRotation,
                        360f * Time.fixedDeltaTime
                    )
                );
            }
        }
    }

    void Jump()
    {
        bodyRigidbody.linearVelocity = new Vector3(
            bodyRigidbody.linearVelocity.x,
            0f,
            bodyRigidbody.linearVelocity.z
        );

        bodyRigidbody.AddForce(
            Vector3.up * jumpForce,
            ForceMode.Impulse
        );

        isGrounded = false;
        canJump = false;
    }

    void CheckGround()
    {
        Vector3 rayOrigin =
            transform.position + Vector3.down * raycastOffset;

        RaycastHit hit;

        if (Physics.Raycast(
            rayOrigin,
            Vector3.down,
            out hit,
            raycastDistance))
        {
            isGrounded = hit.collider.CompareTag("Ground");
        }
        else
        {
            isGrounded = false;
        }

        if (isGrounded && bodyRigidbody.linearVelocity.y <= 0.1f)
        {
            canJump = true;
        }
    }
}
