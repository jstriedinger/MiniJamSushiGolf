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
    private Vector3 _lastForward = Vector3.forward;

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
            // Smooth position follow
            cameraPivot.position = Vector3.Lerp(cameraPivot.position, _currentPlayerBall.transform.position, Time.deltaTime * 4);

            //cameraPivot.position = new Vector3(_currentPlayerBall.transform.position.x, _currentPlayerBall.transform.position.y + 2, _currentPlayerBall.transform.position.z );

            if (_currentPlayerBall.playerState == PlayerState.Aiming)
            {
                // Get flat forward direction (XZ only)
                Vector3 flatForward = Vector3.ProjectOnPlane(_currentPlayerBall.transform.forward, Vector3.up).normalized;

                // If the ball is too tilted or not moving, keep last forward
                if (flatForward.sqrMagnitude > 0.0001f)
                    _lastForward = flatForward;

                // Generate a rotation from flat forward
                Quaternion targetRotation = Quaternion.LookRotation(_lastForward, Vector3.up);

                // Smoothly rotate
                cameraPivot.rotation = Quaternion.Slerp(cameraPivot.rotation, targetRotation, Time.deltaTime * 5);
                
            }
        }
    }


    void ChangePlayer()
    {
        
    }
}
