using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text playerName;
    [SerializeField] private GameObject directionArrow;
    private Sequence _arrowEffect;
    
    [Header("Power bar")]
    public GameObject powerBar;
    private float _maxPower, _minPower = 0;
    private RectTransform _powerBarRectTransform;
    private int powerBarHeight;
    public Image fillBar;
    private RectTransform _fillBarRectTransform;
    
    [Header("Freshness bar")]
    private RectTransform _freshnessBarRectTransform;
    private int _freshnessBarWidth;
    public Image freshnessFillBar;
    private RectTransform _freshnessFillBarRectTransform;
    public static UIManager Instance { get; private set; }

    private void Start()
    {
        StartCoroutine(StartGame());
        
        _arrowEffect = DOTween.Sequence();
        _arrowEffect.Append(directionArrow.transform.DOScaleZ(1.5f, 0.5f));
        _arrowEffect.SetLoops(-1, LoopType.Yoyo);
        _arrowEffect.Pause().SetAutoKill(false);
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
        directionArrow.transform.position = ball.position + (ball.forward * 1.1f) + (ball.up * -0.25f);
    }
    
    public void ShowDirectionArrow(bool show)
    {
        if (directionArrow != null)
        {
            directionArrow.SetActive(show);
            if (show)
            {
                _arrowEffect.Play();
            }
            else
            {
                _arrowEffect.Pause();
            }
        }
    }
    
    public void UpdatePlayerUI(SushiGolfHitControl player)
    {
        if (playerName != null)
        {
            playerName.text = player.name;
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

        _freshnessFillBarRectTransform = freshnessFillBar.GetComponent<RectTransform>();
        
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
    
    public void UpdateFreshnessBar(float currentFreshness)
    {
        Vector2 size = _freshnessFillBarRectTransform.sizeDelta;
        //hard coded the width of the bar but whatever
        size.x = (currentFreshness * 800) / 100;
        _freshnessFillBarRectTransform.sizeDelta = size;
    }

    public void TogglePowerBar(bool b)
    {
        powerBar.gameObject.SetActive(false);
    }
}
