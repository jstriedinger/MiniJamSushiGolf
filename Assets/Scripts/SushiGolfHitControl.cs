using System;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public enum PlayerState
{
    Aiming,
    Rolling
    
}
public class SushiGolfHitControl : MonoBehaviour
{
    private bool _isCurrentPlayer = false;
    public PlayerState playerState = PlayerState.Aiming;
    [Header("Aiming & Hit Settings")]
    private float _minPower, _maxPower, _rotationSpeed, _powerChangeSpeed;
    public Rigidbody rb;

    [Header("UI and Visuals")]
    
    public GameObject directionArrow;

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
            float rotation = Input.GetAxis("Horizontal") * _rotationSpeed * Time.deltaTime;
            transform.Rotate(Vector3.up, rotation);

            if (Input.GetKeyDown(KeyCode.Space) && !isCharging)
            {
                UIManager.Instance?.PositionPowerBar();
                isCharging = true;
                currentPower = _minPower;
                powerIncreasing = true;

                playerState = PlayerState.Aiming;

            }
            
            if (isCharging)
            {
                playerState = PlayerState.Aiming;
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
                    playerState = PlayerState.Rolling;
                    Vector3 hitDirection = transform.forward;
                    rb.AddForce(hitDirection * currentPower, ForceMode.Impulse);

                    isCharging = false;
                    hasHit = true;
                    
                    UIManager.Instance?.TogglePowerBar(false);
                        

                    if (directionArrow != null)
                        directionArrow.SetActive(false);

                    if (hitSounds.Length > 0)
                    {
                        int index = Random.Range(0, hitSounds.Length);
                        oneShotSource.pitch = Random.Range(0.95f, 1.05f);
                        oneShotSource.PlayOneShot(hitSounds[index]);
                    }
                }
            }

            if (directionArrow != null)
            {
                directionArrow.transform.localPosition = new Vector3(0, 0.5f, 1f);
                directionArrow.transform.localRotation = Quaternion.identity;
            }
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
    }
}