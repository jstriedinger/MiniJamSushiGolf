using UnityEngine;

public class FloatingIngredient : MonoBehaviour
{
    public float rotationSpeed = 30f;      // Degrees per second
    public float bobAmplitude = 0.25f;     // Height of the bobbing
    public float bobFrequency = 1f;        // Speed of the bobbing

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        // Y-axis rotation
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);

        // Vertical bobbing
        float newY = startPosition.y + Mathf.Sin(Time.time * bobFrequency) * bobAmplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
