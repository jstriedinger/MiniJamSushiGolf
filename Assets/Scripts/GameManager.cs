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
    
    
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Terrain arena;
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private SushiGolfHitControl[] players;
    public SushiGolfHitControl currentPlayerBall;

    private Vector3 _cameraPivotAngles;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentPlayerBall = players[0];
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


    void ChangePlayer()
    {
        
    }

    public void StartGame(int numPlayers)
    {
        // Set the number of players based on the value passed from the UI
        Debug.Log("Starting game with " + numPlayers + " players.");
        
        // Initialize players array or any other game setup logic here
        players = new SushiGolfHitControl[numPlayers];
        TerrainData terrainData = arena.terrainData;
        Vector3 terrainPosition = arena.transform.position;
        float terrainWidth = terrainData.size.x;
        float terrainLength = terrainData.size.z;
        
        // Create players
        for (int i = 0; i < numPlayers; i++)
        {
            float x = Random.Range(0f, terrainWidth);
            float z = Random.Range(0f, terrainLength);

            float worldX = terrainPosition.x + x;
            float worldZ = terrainPosition.z + z;
            float worldY = arena.SampleHeight(new Vector3(worldX, 0, worldZ)) + terrainPosition.y;

            Vector3 spawnPosition = new Vector3(worldX, worldY, worldZ);
            GameObject newPlayer = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
            SushiGolfHitControl newPlayerSushiControl = newPlayer.GetComponent<SushiGolfHitControl>();
            newPlayerSushiControl.Setup(minPower, maxPower, rotationSpeed, powerChangeSpeed);
            players[i] = newPlayerSushiControl;
        }

        // Set the first player as the current player
        currentPlayerBall = players[0];
        currentPlayerBall.ToggleIsCurrentPlayer(true);
    }
}
