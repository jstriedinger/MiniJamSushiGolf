using System.Collections.Generic;
using UnityEngine;

public class ScoreControl : MonoBehaviour
{
    public Dictionary<string, int> ingredientCountCombinations = new Dictionary<string, int>();
    public Dictionary<string, float> scoreData = new Dictionary<string, float>();

    public float freshness;
    public string customerName;
    private void Start()
    {
        InitializeDicts();
    }

    private void InitializeDicts()
    {
        ingredientCountCombinations.Add("Avocado", 0);
        ingredientCountCombinations.Add("Bug", 0);
        ingredientCountCombinations.Add("Crab", 0);
        ingredientCountCombinations.Add("Cucumber", 0);
        ingredientCountCombinations.Add("Fish", 0);
        ingredientCountCombinations.Add("Nori", 0);
        ingredientCountCombinations.Add("Poop", 0);
        ingredientCountCombinations.Add("Shrimp", 0);
        ingredientCountCombinations.Add("Wood", 0);
        
        scoreData.Add("Avocado", 0);
        scoreData.Add("Bug", 0);
        scoreData.Add("Crab", 0);
        scoreData.Add("Cucumber", 0);
        scoreData.Add("Fish", 0);
        scoreData.Add("Nori", 0);
        scoreData.Add("Poop", 0);
        scoreData.Add("Shrimp", 0);
        scoreData.Add("Wood", 0);
    }
    
    public void AddToScoreCombo(string n, float s)
    {
        scoreData[n] = s;
    }

    public void AddToCount(string n)
    {
        if (ingredientCountCombinations.ContainsKey(n))
        {
            ingredientCountCombinations[n] += 1;
        }
        else
        {
            ingredientCountCombinations[n] = 1;
        }
    }

    public void CalculateScore()
    {
        SendScore();
    }

    private void SendScore()
    {
        ScoreManager.OnReceiveNewScore.Invoke(gameObject.name, this);
    }
    
}
