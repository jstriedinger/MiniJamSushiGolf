using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject directionArrow;
    [Header("Power bar")]
    public GameObject powerBar;
    private float _maxPower, _minPower = 0;
    private RectTransform _powerBarRectTransform;
    private int powerBarHeight;
    public Image fillBar;
    private RectTransform _fillBarRectTransform;
    public static UIManager Instance { get; private set; }

    private void Start()
    {
        StartCoroutine(StartGame());
    }

    private void Awake() 
    { 
        // If there is an instance, and it's not me, delete myself.
    
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        } 
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Update is called once per frame
    void Update()
    {
        if(GameManager.Instance?.currentPlayerBall != null && GameManager.Instance?.currentPlayerBall.playerState == PlayerState.Aiming)
        {
            PointDirectionArrow();
        }
    }
    
    private void PointDirectionArrow()
    {
        Vector3 direction = GameManager.Instance.currentPlayerBall.transform.forward;
        directionArrow.transform.rotation = Quaternion.LookRotation(direction);
        //position the arrow near ball
        Transform ball = GameManager.Instance.currentPlayerBall.transform;
        directionArrow.transform.position = ball.position + (ball.forward * 2);
    }
    
    public void ShowDirectionArrow(bool show)
    {
        if (directionArrow != null)
        {
            directionArrow.SetActive(show);
        }
    }

    IEnumerator StartGame()
    {
        yield return new WaitForSeconds(0.5f);
        //because index starts at 0
        GameManager.Instance?.StartGame();
        
        _fillBarRectTransform = fillBar.GetComponent<RectTransform>();
        _powerBarRectTransform = powerBar.GetComponent<RectTransform>();
        powerBarHeight = (int)_powerBarRectTransform.sizeDelta.y;
        
        _maxPower = GameManager.Instance.maxPower;
        _minPower = GameManager.Instance.minPower;
    }

    public void PositionPowerBar()
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(GameManager.Instance.currentPlayerBall.transform.position) + new Vector3(50, 1, 0);
        _powerBarRectTransform.position = screenPos;

        if (powerBar != null)
        {
            powerBar.gameObject.SetActive(true);
            Vector2 size = _fillBarRectTransform.sizeDelta;
            size.y = _minPower;
            _fillBarRectTransform.sizeDelta = size;
        }
    }

    public void UpdatePowerBar(float currentPower)
    {
        Vector2 size = _fillBarRectTransform.sizeDelta;
        size.y = (currentPower * powerBarHeight) / _maxPower;
        _fillBarRectTransform.sizeDelta = size;
    }

    public void TogglePowerBar(bool b)
    {
        powerBar.gameObject.SetActive(false);
    }
}
