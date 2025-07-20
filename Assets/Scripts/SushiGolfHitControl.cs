using System;
using System.Collections;
using PlayerProperty;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public enum PlayerState
{
    None,
    Aiming,
    Charging,
    Rolling
}

public class SushiGolfHitControl : MonoBehaviour
{
    private bool _isCurrentPlayer = false;
    public PlayerState playerState = PlayerState.Charging;

    [Header("Aiming & Hit Settings")]
    private float _minPower, _maxPower, _rotationSpeed, _powerChangeSpeed;
    public Rigidbody rb;

    [Header("Audio Clips")]
    public AudioClip[] hitSounds;
    public AudioClip rollLoopSound;
    public AudioClip[] pickupSounds;
    public AudioClip[] fartSounds;

    private AudioSource oneShotSource;
    private AudioSource rollLoopSource;

    private float currentPower = 0f;
    private bool powerIncreasing = true;
    private bool hasHit = false;
    private bool _canCheckStopRolling = false;
    private bool isFirstTurn = true;
    private float _currentYaw = 0f;
    public bool hasFinished = false;

    private void Start()
    {
        oneShotSource = gameObject.AddComponent<AudioSource>();
        rollLoopSource = gameObject.AddComponent<AudioSource>();
        rollLoopSource.clip = rollLoopSound;
        rollLoopSource.loop = true;
        rollLoopSource.volume = 0.5f;
        rollLoopSource.playOnAwake = false;
    }

    void Update()
    {
        if (hasHit)
        {
            float speed = rb.linearVelocity.magnitude;
            if (speed > 0.5f)
            {
                if (!rollLoopSource.isPlaying)
                    rollLoopSource.Play();
            }
            else
            {
                if (rollLoopSource.isPlaying)
                    rollLoopSource.Stop();
            }

            return;
        }

        if (_isCurrentPlayer)
        {
            switch (playerState)
            {
                case PlayerState.Aiming:
                    HandlePlayerAiming();
                    break;
                case PlayerState.Charging:
                    HandlePlayerCharging();
                    break;
            }
        }
    }

    private void FixedUpdate()
    {
        if (_isCurrentPlayer && playerState == PlayerState.Rolling)
        {
            HandlePlayerRolling();
        }
    }

    IEnumerator StartRolling()
    {
        _currentYaw = 0;
        if (isFirstTurn)
        {
            isFirstTurn = false;
            rb.useGravity = true;
        }

        playerState = PlayerState.Rolling;
        Vector3 hitDirection = transform.forward;
        rb.AddForce(hitDirection * currentPower, ForceMode.Impulse);

        hasHit = true;
        UIManager.Instance?.TogglePowerBar(false);

        if (hitSounds.Length > 0)
        {
            int index = Random.Range(0, hitSounds.Length);
            oneShotSource.pitch = Random.Range(0.95f, 1.05f);
            oneShotSource.PlayOneShot(hitSounds[index]);
        }

        yield return new WaitForSeconds(0.5f);
        _canCheckStopRolling = true;
    }

    private void HandlePlayerRolling()
    {
        if (rb.linearVelocity.sqrMagnitude < 0.1f && _canCheckStopRolling)
        {
            hasHit = false;
            playerState = PlayerState.None;
            GameManager.Instance.ChangePlayer();
            _canCheckStopRolling = false;
        }
    }

    private void HandlePlayerAiming()
    {
        float rotationInput = Input.GetAxis("Horizontal") * _rotationSpeed * Time.deltaTime;
        _currentYaw += rotationInput;
        transform.rotation = Quaternion.Euler(0f, _currentYaw, 0f);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            UIManager.Instance?.PositionPowerBar();
            currentPower = _minPower;
            powerIncreasing = true;

            playerState = PlayerState.Charging;
            UIManager.Instance?.ShowDirectionArrow(false);
        }
    }

    private void HandlePlayerCharging()
    {
        if (powerIncreasing)
        {
            currentPower += _powerChangeSpeed * Time.deltaTime;
            if (currentPower >= _maxPower)
            {
                currentPower = _maxPower;
                powerIncreasing = false;
            }
        }
        else
        {
            currentPower -= _powerChangeSpeed * Time.deltaTime;
            if (currentPower <= _minPower)
            {
                currentPower = _minPower;
                powerIncreasing = true;
            }
        }

        UIManager.Instance?.UpdatePowerBar(currentPower);

        if (Input.GetKeyUp(KeyCode.Space))
        {
            StartCoroutine(StartRolling());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ingredient"))
        {
            Rigidbody ingredientRb = other.gameObject.GetComponent<Rigidbody>();

            // Freeze Rigidbody and remove physical collider interaction
            if (ingredientRb != null)
                ingredientRb.isKinematic = true;

            other.enabled = false; // Still works if it's just a trigger

            // Disable FloatingIngredient bobbing/rotating
            FloatingIngredient floatScript = other.gameObject.GetComponent<FloatingIngredient>();
            if (floatScript != null)
            {
                floatScript.enabled = false;
            }

            // Parent the ingredient to the sushi ball and slightly randomize position
            float sphereRadius = GetComponent<SphereCollider>().radius * transform.lossyScale.x;
            float safeOffset = 0.025f; // small buffer to place it outside
            Vector3 directionOut = (other.transform.position - transform.position).normalized;
            Vector3 contactPoint = transform.position + directionOut * (sphereRadius + safeOffset);
            //Vector3 directionOut = (contactPoint - transform.position).normalized;
            //Vector3 contactPoint = GetComponent<Collider>().ClosestPoint(other.transform.position);
            other.transform.position = contactPoint;
            other.transform.rotation = Quaternion.LookRotation(directionOut);
            
            other.transform.SetParent(transform);
            //other.transform.localPosition = localPoint;
            //other.transform.localRotation = Random.rotation;
            //other.transform.localPosition += Random.insideUnitSphere * 0.1f;

            // 🔊 Play pickup sound
            if (pickupSounds.Length > 0)
            {
                int index = Random.Range(0, pickupSounds.Length);
                oneShotSource.pitch = Random.Range(0.95f, 1.05f);
                oneShotSource.PlayOneShot(pickupSounds[index]);
            }

            // 💨 Fart if poop
            if (other.gameObject.name.Contains("Poop") && fartSounds.Length > 0)
            {
                int index = Random.Range(0, fartSounds.Length);
                oneShotSource.pitch = Random.Range(0.9f, 1.1f);
                oneShotSource.PlayOneShot(fartSounds[index]);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ingredient"))
        {
            Rigidbody ingredientRb = collision.rigidbody;
            Collider ingredientCol = collision.collider;

            // Freeze Rigidbody and remove physical collider interaction
            if (ingredientRb != null)
                ingredientRb.isKinematic = true;

            if (ingredientCol != null)
                ingredientCol.enabled = false; // Still works if it's just a trigger

            // Disable FloatingIngredient bobbing/rotating
            FloatingIngredient floatScript = collision.gameObject.GetComponent<FloatingIngredient>();
            if (floatScript != null)
            {
                floatScript.enabled = false;
            }

            // Parent the ingredient to the sushi ball and slightly randomize position
            collision.transform.SetParent(transform);
            ContactPoint contact = collision.contacts[0];
            Vector3 localPoint = transform.InverseTransformPoint(contact.point);
            collision.transform.localPosition = localPoint;
            collision.transform.localRotation = Random.rotation;
            collision.transform.localPosition += Random.insideUnitSphere * 0.1f;

            // 🔊 Play pickup sound
            if (pickupSounds.Length > 0)
            {
                int index = Random.Range(0, pickupSounds.Length);
                oneShotSource.pitch = Random.Range(0.95f, 1.05f);
                oneShotSource.PlayOneShot(pickupSounds[index]);
            }

            // 💨 Fart if poop
            if (collision.gameObject.name.Contains("Poop") && fartSounds.Length > 0)
            {
                int index = Random.Range(0, fartSounds.Length);
                oneShotSource.pitch = Random.Range(0.9f, 1.1f);
                oneShotSource.PlayOneShot(fartSounds[index]);
            }
        }
    }

    public void Setup(float minPower, float maxPower, float rotationSpeed, float powerChangeSpeed)
    {
        _minPower = minPower;
        _maxPower = maxPower;
        _rotationSpeed = rotationSpeed;
        _powerChangeSpeed = powerChangeSpeed;
    }

    public void ToggleIsCurrentPlayer(bool isCurrentPlayer)
    {
        _isCurrentPlayer = isCurrentPlayer;
        GetComponent<Freshness>().ModifyFreshness(isCurrentPlayer);
        playerState = PlayerState.Aiming;
        _currentYaw = transform.eulerAngles.y;
        rb.linearVelocity = Vector3.zero;
    }
}
