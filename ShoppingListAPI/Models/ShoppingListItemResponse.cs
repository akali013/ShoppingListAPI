namespace ShoppingListAPI.Models
{
    public class ShoppingListItemResponse
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public Boolean IsChecked { get; set; }

        public ShoppingListItemResponse(string name, int quantity, Boolean isChecked) 
        {
            Name = name;
            Quantity = quantity;
            IsChecked = isChecked;
        }
    }
}
