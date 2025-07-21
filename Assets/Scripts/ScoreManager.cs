using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static Action OnGameOverCalculateScore;
    public static Action<string, ScoreControl> OnReceiveNewScore;
    public Dictionary<string, ScoreControl> playerScores = new Dictionary<string, ScoreControl>();
    public GameObject scoreCanvas;
    public List<ScoreData> scoreDisplays = new List<ScoreData>();
    
    private void Awake()
    {
        OnGameOverCalculateScore += DisplayFinalScore;
        OnReceiveNewScore += ReceiveNewScore;
    }

    public void DisplayFinalScore()
    {
        scoreCanvas.gameObject.SetActive(true);
        for (int i = 0 ; i < playerScores.Count ; i++)
        {
            scoreDisplays[i].gameObject.SetActive(true);
            scoreDisplays[i].DisplayScore(playerScores.Values.ToList()[i]);
        }
    }
    
    public void ReceiveNewScore(string playerName, ScoreControl score)
    {
        playerScores.TryAdd(playerName, score);
        // Debug.Log("player scores count " + playerScores.Count);
    }
    
}
