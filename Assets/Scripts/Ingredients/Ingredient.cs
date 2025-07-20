using UnityEngine;

namespace Ingredients
{
    public class Ingredient: MonoBehaviour, IngredientInterface
    {
        [SerializeField] private string ingredientName;
        
        public string Name => ingredientName;
        public bool pickedup;

        public bool PickedUp
        {
            get => pickedup;
            set => pickedup = value;
        }
        
        public void OnPickUp()
        {
            if (!PickedUp)
            {
                PickedUp = true;
                //Debug.Log($"{ingredientName} Picked Up");
            }
        }
    }
}