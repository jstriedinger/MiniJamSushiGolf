namespace Ingredients
{
    public interface IngredientInterface
    {
        string Name { get; }
        bool PickedUp { get; set; }
        void OnPickUp();
    }
}