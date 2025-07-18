using UnityEngine;
using UnityEngine.UI;

public class SushiGolfHitControl : MonoBehaviour
{
    [Header("Aiming & Hit Settings")]
    public float rotationSpeed = 100f;
    public float minPower = 5f;
    public float maxPower = 20f;
    public float powerChangeSpeed = 2f;
    public Rigidbody rb;

    [Header("UI and Visuals")]
    public Slider powerBar;
    public GameObject directionArrow;

    private bool isCharging = false;
    private float currentPower = 0f;
    private bool powerIncreasing = true;
    private bool hasHit = false;

    void Update()
    {
        if (hasHit) return;

        // --- Aiming ---
        float rotation = Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime;
        transform.Rotate(Vector3.up, rotation);

        // --- Charging ---
        if (Input.GetKeyDown(KeyCode.Space) && !isCharging)
        {
            isCharging = true;
            currentPower = minPower;
            powerIncreasing = true;

            if (powerBar != null)
            {
                powerBar.gameObject.SetActive(true);
                powerBar.value = 0f;
            }
        }

        // --- Power fluctuation while holding ---
        if (isCharging)
        {
            if (powerIncreasing)
            {
                currentPower += powerChangeSpeed * Time.deltaTime;
                if (currentPower >= maxPower)
                {
                    currentPower = maxPower;
                    powerIncreasing = false;
                }
            }
            else
            {
                currentPower -= powerChangeSpeed * Time.deltaTime;
                if (currentPower <= minPower)
                {
                    currentPower = minPower;
                    powerIncreasing = true;
                }
            }

            if (powerBar != null)
            {
                powerBar.value = (currentPower - minPower) / (maxPower - minPower);
            }

            // --- Release hit on second spacebar press ---
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Vector3 hitDirection = transform.forward;
                rb.AddForce(hitDirection * currentPower, ForceMode.Impulse);

                isCharging = false;
                hasHit = true;

                if (powerBar != null)
                    powerBar.gameObject.SetActive(false);

                if (directionArrow != null)
                    directionArrow.SetActive(false);
            }
        }

        // Keep arrow pointing forward
        if (directionArrow != null)
        {
            directionArrow.transform.localPosition = new Vector3(0, 0.5f, 1f);
            directionArrow.transform.localRotation = Quaternion.identity;
        }
    }

    // 🍣 Katamari-style ingredient sticking logic
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ingredient"))
        {
            // Get the Rigidbody and Collider
            Rigidbody ingredientRb = collision.rigidbody;
            Collider ingredientCol = collision.collider;

            if (ingredientRb != null)
            {
                ingredientRb.isKinematic = true;
            }

            if (ingredientCol != null)
            {
                ingredientCol.enabled = false; // Disable future collisions
            }

            // Stick to rice ball
            collision.transform.SetParent(transform);

            // Position it at point of contact
            ContactPoint contact = collision.contacts[0];
            Vector3 localPoint = transform.InverseTransformPoint(contact.point);
            collision.transform.localPosition = localPoint;

            // Optional: add some rotation and offset for more organic feel
            collision.transform.localRotation = Random.rotation;
            collision.transform.localPosition += Random.insideUnitSphere * 0.1f;
        }
    }
}
