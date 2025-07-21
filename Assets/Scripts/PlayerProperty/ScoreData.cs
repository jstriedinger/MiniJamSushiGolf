using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class ScoreData : MonoBehaviour
{
    public List<TMP_Text> ingredientNames;
    public List<TMP_Text> quantities;
    public List<TMP_Text> scores;

    public TMP_Text finalScore;
    public TMP_Text customerName;
    public TMP_Text freshness;
    
    public void DisplayScore(ScoreControl scoreControl)
    {
        for (int i = 0; i < ingredientNames.Count; i++)
        {
            var CountValues = scoreControl.ingredientCountCombinations.Values.ToList();
            var CountKeys = scoreControl.ingredientCountCombinations.Keys.ToList();
            var ScoreValues = scoreControl.scoreData.Values.ToList();
            int ingredientCount = CountValues[i];
            string ingredientName = CountKeys[i];
            float score = ScoreValues[i];
            ingredientNames[i].text = ingredientName;
            quantities[i].text = ingredientCount.ToString();
            scores[i].text = score.ToString();
        }
        customerName.text = scoreControl.customerName;
        freshness.text = scoreControl.freshness.ToString("F2");
        CalculateFinalScore(scoreControl);
    }

    private void CalculateFinalScore(ScoreControl scoreControl)
    {
        float totalScore = 0;
        foreach (KeyValuePair<string, int> pair in scoreControl.ingredientCountCombinations)
        {
            string key = pair.Key;
            int valueCount = pair.Value;

            float valueScore = scoreControl.scoreData[key];
            
            float scoreIncrement = valueCount * valueScore;
            totalScore += scoreIncrement;
        }
        finalScore.text = totalScore.ToString();
    }
}
