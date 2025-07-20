using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
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
    [Header("Gameplay settings")]
    public float minPower = 4f;
    public float maxPower = 20f;
    public float rotationSpeed = 100f;
    public float powerChangeSpeed = 8f;
    [SerializeField] private Transform[] initialPlayerPositions;
    
    [Header("Items")]
    [SerializeField] private GameObject[] itemsToSpawn;
    private GameObject[] _spawnedItems;
    [SerializeField] private GameObject itemsPositionsObj;
    private Transform[] _itemsPositions;
    
    
    [Header("Gameover management")]
    
    
    [Header("Characters & camera")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform cameraPivot;
    private SushiGolfHitControl[] _players;
    public SushiGolfHitControl currentPlayerBall;
    private int _currentPlayerIndex =-1;

    private Vector3 _cameraPivotAngles;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _cameraPivotAngles = cameraPivot.eulerAngles;
        
    }

    // Update is called once per frame
    void Update()
    {
        //camera pivot follow current player ball
        if(currentPlayerBall != null)
        {
            cameraPivot.position = Vector3.Lerp(cameraPivot.position, currentPlayerBall.transform.position, Time.deltaTime * 4);

            if (currentPlayerBall.playerState == PlayerState.Aiming)
            {
                _cameraPivotAngles = currentPlayerBall.transform.eulerAngles;
                cameraPivot.rotation = Quaternion.Euler(15f, _cameraPivotAngles.y, 0f);
            }

        }
    }


    public void ChangePlayer()
    {
        Debug.Log("Changing player");
        //calcualte points
        
        //change player
        if (currentPlayerBall != null)
        {
            currentPlayerBall.ToggleIsCurrentPlayer(false);
        }

        do
        {
            _currentPlayerIndex++;
            if(_currentPlayerIndex >= _players.Length)
            {
                _currentPlayerIndex = 0; 
            }
            
        } while (_players[_currentPlayerIndex].hasFinished);
        //we must change until we find the nxt player that has not finished
        currentPlayerBall = null;
        //Move out camera pivot
        Sequence seq = DOTween.Sequence();
        seq.Append(cameraPivot.DOMove(_players[_currentPlayerIndex].transform.position, 1f));
        seq.Join( cameraPivot.DORotate(new Vector3(15f, _players[_currentPlayerIndex].transform.eulerAngles.y, 0f), 1f))
        .OnComplete(() =>
        {
            currentPlayerBall = _players[_currentPlayerIndex];
            currentPlayerBall.ToggleIsCurrentPlayer(true);
            UIManager.Instance?.ShowDirectionArrow(true);
        });
        
    }

    public void StartGame(int numPlayers)
    {
        // Set the number of players based on the value passed from the UI
        Debug.Log("Starting game with " + numPlayers + " players.");
        
        // Initialize players array or any other game setup logic here
        _players = new SushiGolfHitControl[numPlayers];
        
        // Create players
        for (int i = 0; i < numPlayers; i++)
        {
            GameObject newPlayer = Instantiate(playerPrefab, initialPlayerPositions[i].position,initialPlayerPositions[i].rotation);
            //make our ball not move
            Rigidbody rb = newPlayer.GetComponent<Rigidbody>();
            rb.useGravity = false;
            SushiGolfHitControl newPlayerSushiControl = newPlayer.GetComponent<SushiGolfHitControl>();
            newPlayerSushiControl.Setup(minPower, maxPower, rotationSpeed, powerChangeSpeed);
            _players[i] = newPlayerSushiControl;
        }

        // Set the first player as the current player
        ChangePlayer();
        
        //initialize items positions
        _itemsPositions = itemsPositionsObj.transform.Cast<Transform>()
            .OrderBy(t => UnityEngine.Random.value)
            .ToArray();
        //now lets go throguh them and put our items
        _spawnedItems = new GameObject[itemsToSpawn.Length];
        for (int i = 0; i < _itemsPositions.Length; i++)
        {
            if (i < itemsToSpawn.Length)
            {
                Vector3 itemPosition = _itemsPositions[i].position;
                Ray ray = new Ray(_itemsPositions[i].position, Vector3.down);

                if (Physics.Raycast(ray, out RaycastHit hit, 50f))
                {
                    itemPosition = hit.point + Vector3.up * 1f;
                }
                GameObject item = Instantiate(itemsToSpawn[i],itemPosition ,Quaternion.identity);
                _spawnedItems[i] = item;
            }
        }
    }

    //Trigerred when all players have finished the hole
    public void HandleGameOver()
    {
        Debug.Log("Gameover");
        //we gotta put the camera somewhere, for not just zero
        currentPlayerBall.ToggleIsCurrentPlayer(false);
        currentPlayerBall = null;
        _currentPlayerIndex = -1;
        Sequence seq = DOTween.Sequence();
        seq.Append(cameraPivot.DOMove(new Vector3(10,10,10), 5f));
        seq.Join( cameraPivot.DORotate(new Vector3(45,30,0), 5f))
            .OnComplete(() =>
            {
                //Shwo game over UI
            });
    }

    public void OnBallHitHole(GameObject otherGameObject)
    {
        SushiGolfHitControl hitPlayer = otherGameObject.GetComponent<SushiGolfHitControl>();
        //remove this player from the game
        hitPlayer.hasFinished = true;
        Rigidbody rb = otherGameObject.GetComponent<Rigidbody>();
        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = true;
        //lets check if all players have finished

    }

    public void TryToChangePlayer()
    {
        bool allFinished = true;
        foreach (SushiGolfHitControl player in _players)
        {
            if (!player.hasFinished)
            {
                allFinished = false;
                break;
            }
        }

        if (allFinished)
            HandleGameOver();
        else
            ChangePlayer();
    }
}
