using UnityEngine;

public class FloatingIngredient : MonoBehaviour
{
    public float rotationSpeed = 30f;
    public float bobAmplitude = 0.25f;
    public float bobFrequency = 1f;

    private Vector3 startPosition;
    private bool isCollected = false;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        if (isCollected) return;

        // Rotate around Y axis
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);

        // Bob up and down
        float newY = startPosition.y + Mathf.Sin(Time.time * bobFrequency) * bobAmplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    public void StopFloating()
    {
        isCollected = true;
    }
}