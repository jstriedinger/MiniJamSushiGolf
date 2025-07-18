using System;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SushiGolfHitControl : MonoBehaviour
{
    [Header("Aiming & Hit Settings")]
    public float rotationSpeed = 100f;
    public float minPower = 5f;
    public float maxPower = 100f;
    public float powerChangeSpeed = 2f;
    public Rigidbody rb;

    [Header("UI and Visuals")]
    public GameObject powerBar;
    private RectTransform _powerBarRectTransform;
    private int powerBarHeight;
    public Image fillBar;
    private RectTransform _fillBarRectTransform;
    public GameObject directionArrow;

    private bool isCharging = false;
    private float currentPower = 0f;
    private bool powerIncreasing = true;
    private bool hasHit = false;

    private void Start()
    {
        _fillBarRectTransform = fillBar.GetComponent<RectTransform>();
        
        //lets define our scale between power and actual bar size
        _powerBarRectTransform = powerBar.GetComponent<RectTransform>();
        powerBarHeight = (int)_powerBarRectTransform.sizeDelta.y; 
        Debug.Log(powerBarHeight);
        

    }

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

            //put the powerbar next to the ball
            Vector3 worldPos = transform.position + new Vector3(10,10);
            Vector3 screenPos = (Camera.main.WorldToScreenPoint(transform.position)) + new Vector3(50,1,0);
            Debug.Log(screenPos);
            _powerBarRectTransform.position = screenPos;
            
            if (powerBar != null)
            {
                powerBar.gameObject.SetActive(true);
               
                Vector2 size = _fillBarRectTransform.sizeDelta;
                size.y = minPower;  // e.g., 100f
                _fillBarRectTransform.sizeDelta = size;
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
                //assuming the same scale of 100 for now jut to test
                Vector2 size = _fillBarRectTransform.sizeDelta;
                size.y = (currentPower*powerBarHeight) / maxPower;  // e.g., 100f
                _fillBarRectTransform.sizeDelta = size;
                //powerBar.value = (currentPower - minPower) / (maxPower - minPower);
            }

            // --- Release hit on second spacebar press ---
            if (Input.GetKeyUp(KeyCode.Space))
            {
                Vector3 hitDirection = transform.forward;
                rb.AddForce(hitDirection * currentPower, ForceMode.Impulse);

                isCharging = false;
                hasHit = true;

                if (fillBar != null)
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
