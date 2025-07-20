using System.Collections.Generic;
using Ingredients;
using UnityEngine;

namespace PlayerProperty
{
    public class IngredientControl: MonoBehaviour
    {
        public List<Ingredient> ingredients;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Ingredient"))
            {
                AddIngredient(other.GetComponent<Ingredient>());
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Ingredient"))
            {
                AddIngredient(collision.gameObject.GetComponent<Ingredient>());
            }
        }
        
        public void AddIngredient(Ingredient ingredient)
        {
            ingredients.Add(ingredient);
            ingredient.OnPickUp();
        }

        public List<Ingredient> GetIngredients()
        {
            return ingredients;
        }
    }
}