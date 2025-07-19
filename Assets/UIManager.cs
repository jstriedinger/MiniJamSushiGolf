using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Power bar")]
    public GameObject powerBar;
    private float _maxPower, _minPower = 0;
    private RectTransform _powerBarRectTransform;
    private int powerBarHeight;
    public Image fillBar;
    private RectTransform _fillBarRectTransform;
    
    
    [SerializeField] private GameObject gameSelectScreen;
    [SerializeField] private TMP_Dropdown numPlayersDropdown;
    private int _numPlayers;
    public static UIManager Instance { get; private set; }

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
    void Start()
    {
        numPlayersDropdown.onValueChanged.AddListener(OnNumPlayersChanged);
    }
    
    private void OnNumPlayersChanged(int value)
    {
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void StartGame()
    {
        Destroy(gameSelectScreen);
        //because index starts at 0
        GameManager.Instance?.StartGame(numPlayersDropdown.value + 1);
        
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
