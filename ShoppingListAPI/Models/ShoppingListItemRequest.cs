using System.ComponentModel.DataAnnotations;

namespace ShoppingListAPI.Models
{
    public class ShoppingListItemRequest
    {
        public string Name { get; set; }
        public int Quantity { get; set; } = 1;
        public Boolean IsChecked { get; set; } = false;
    }
}
