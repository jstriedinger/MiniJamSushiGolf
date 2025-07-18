using UnityEngine;

public class RiceBallController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float rotationSpeed = 5f;
    private Rigidbody rb;

    void Start() => rb = GetComponent<Rigidbody>();

    void FixedUpdate()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        Vector3 force = new Vector3(moveX, 0, moveZ) * moveSpeed;
        rb.AddForce(force);
    }
}