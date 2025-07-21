using System;
using System.Collections.Generic;
using Ingredients;
using PlayerProperty;
using UnityEngine;

namespace Customers
{
    public class Customer : MonoBehaviour, CustomerInterface
    {
        public string customerName;
        public Dictionary<string, float> scoreDict = new Dictionary<string, float>();

        public List<string> ingredientNames = new List<string>();
        public List<float> ingredientScores = new List<float>();

        public bool ateAlready = false;
        private float eatTimer = 0f;
        private float eatCoolDown = 1f;
        private void Awake()
        {
            InitializeCustomer();
        }

        private void Update()
        {
            if (ateAlready)
            {
                if (eatTimer < eatCoolDown)
                {
                    eatTimer += Time.deltaTime;
                }
                else if(eatTimer > eatCoolDown)
                {
                    eatTimer = 0f;
                    ateAlready = false;
                }
            }
        }


        public void OnEat(IngredientControl iC)
        {
            List<Ingredient> allEatenIngredients = iC.GetIngredients();
            
            foreach (Ingredient ingredient in allEatenIngredients)
            {
                iC.GetComponent<ScoreControl>().AddToCount(ingredient.Name);
                iC.GetComponent<ScoreControl>().AddToScoreCombo(ingredient.Name, GetIngredientScore(ingredient));
            }
            iC.GetComponent<ScoreControl>().CalculateScore();
            iC.GetComponent<ScoreControl>().freshness = iC.gameObject.GetComponent<Freshness>().freshnessAmount;
            iC.GetComponent<ScoreControl>().customerName = customerName;
        }

        public float GetIngredientScore(Ingredient ingredient)
        {
            if (scoreDict.TryGetValue(ingredient.Name, out float value))
            {
                return value;
            }
            else
            {
                return 0;
            }
        }

        private void InitializeCustomer()
        {
            for (int i = 0; i < ingredientNames.Count && i < ingredientScores.Count; i++)
            {
                string name = ingredientNames[i];
                float score = ingredientScores[i];

                // Add or update the dictionary
                scoreDict[name] = score;
            }

            //Debug.Log(scoreDict.Count);
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<IngredientControl>() && !ateAlready)
            {

                Debug.Log("oneat");
                OnEat(other.gameObject.GetComponent<IngredientControl>());
                ateAlready = true;
            }
        }
    }
}