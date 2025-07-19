using System;
using System.Collections;
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

    private bool isCharging = false;
    private float currentPower = 0f;
    private bool powerIncreasing = true;
    private bool hasHit = false;
    private bool _canCheckStopRolling = false;
    bool isFirstTurn = true;
    private float _currentYaw = 0f;

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

        //This player is the active one
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
        if (_isCurrentPlayer)
        {
            switch (playerState)
            {
                case PlayerState.Rolling:
                    HandlePlayerRolling();
                    break;

            }
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
    
    //On the first turn our ball is static
    IEnumerator HandleFirstTurnKick()
    {
        rb.isKinematic = false;
        yield return new WaitForFixedUpdate(); // Wait until physics update
        rb.AddForce(Vector3.forward * 10f, ForceMode.Impulse);
    }

    private void HandlePlayerRolling()
    {
        //we need to detect when it stops rolling
        //maybe with velocity is near zero?
        if (rb.linearVelocity.sqrMagnitude < 0.1f && _canCheckStopRolling)
        {
            hasHit = false;
            playerState = PlayerState.None;
            //tell game manager is time to change player
            GameManager.Instance.ChangePlayer();
            _canCheckStopRolling = false;
        }
        
    }

    private void HandlePlayerAiming()
    {
        
        float rotationInput = Input.GetAxis("Horizontal") * _rotationSpeed * Time.deltaTime;
        _currentYaw += rotationInput;
        // Apply to ball player
        transform.rotation = Quaternion.Euler(0f, _currentYaw, 0f);

        // Now lets point the stupid arrow
        //Vector3 lookDirection = Quaternion.Euler(0f, _currentYaw, 0f) * Vector3.forward;
        //directionArrow.transform.rotation = Quaternion.LookRotation(lookDirection, Vector3.up);
        
        //float rotation = Input.GetAxis("Horizontal") * _rotationSpeed * Time.deltaTime;
        //transform.Rotate(Vector3.up, rotation, Space.World);

        if (Input.GetKeyDown(KeyCode.Space) )
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ingredient"))
        {
            Rigidbody ingredientRb = collision.rigidbody;
            Collider ingredientCol = collision.collider;

            if (ingredientRb != null)
                ingredientRb.isKinematic = true;

            if (ingredientCol != null)
                ingredientCol.enabled = false;

            collision.transform.SetParent(transform);

            ContactPoint contact = collision.contacts[0];
            Vector3 localPoint = transform.InverseTransformPoint(contact.point);
            collision.transform.localPosition = localPoint;

            collision.transform.localRotation = Random.rotation;
            collision.transform.localPosition += Random.insideUnitSphere * 0.1f;

            // 🔊 Play random pickup sound
            if (pickupSounds.Length > 0)
            {
                int index = Random.Range(0, pickupSounds.Length);
                oneShotSource.pitch = Random.Range(0.95f, 1.05f);
                oneShotSource.PlayOneShot(pickupSounds[index]);
            }

            // 💨 Play random fart sound if it's poop
            if (collision.gameObject.name.Contains("Poop") && fartSounds.Length > 0)
            {
                int index = Random.Range(0, fartSounds.Length);
                oneShotSource.pitch = Random.Range(0.9f, 1.1f);
                oneShotSource.PlayOneShot(fartSounds[index]);
            }
        }
    }

    //Setu from game manager
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
        playerState = PlayerState.Aiming;
        _currentYaw = transform.eulerAngles.y;
        rb.linearVelocity = Vector3.zero;
    }
}