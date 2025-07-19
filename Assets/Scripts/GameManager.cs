using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private SushiGolfHitControl[] players;
    private SushiGolfHitControl _currentPlayerBall;
    public SushiGolfHitControl CurrentPlayerBall
    {
        get => _currentPlayerBall;
        set => _currentPlayerBall = value;
    }

    private Vector3 _cameraPivotAngles;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _currentPlayerBall = players[0];
        _cameraPivotAngles = cameraPivot.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        //camera pivot follow current player ball
        if(_currentPlayerBall != null)
        {
            cameraPivot.position = Vector3.Lerp(cameraPivot.position, _currentPlayerBall.transform.position, Time.deltaTime * 4);


            if (_currentPlayerBall.playerState == PlayerState.Aiming)
            {
                _cameraPivotAngles = _currentPlayerBall.transform.eulerAngles;
                cameraPivot.rotation = Quaternion.Euler(15f, _cameraPivotAngles.y, 0f);
            }

        }
    }


    void ChangePlayer()
    {
        
    }
}
