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
        private void Awake()
        {
            InitializeCustomer();
        }
        
        
        public void OnEat(IngredientControl iC)
        {
            List<Ingredient> allEatenIngredients = iC.GetIngredients();
            float totalScore = 0f;
            foreach (Ingredient ingredient in allEatenIngredients)
            {
                totalScore += GetIngredientScore(ingredient);
            }

            //Debug.Log(totalScore);
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
            for (int i = 0 ; i < ingredientNames.Count; i++)
            {
                scoreDict.Add(ingredientNames[i], ingredientScores[i]);
            }

            //Debug.Log(scoreDict.Count);
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<IngredientControl>())
            {
                //Debug.Log($"{customerName} is eating");
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.GetComponent<IngredientControl>())
            {
                OnEat(other.gameObject.GetComponent<IngredientControl>());
            }
        }
    }
}