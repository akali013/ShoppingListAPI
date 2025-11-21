using System.ComponentModel.DataAnnotations;

namespace ShoppingListAPI.Models
{
    public class ItemRequest
    {
        [Required]
        public string Name { get; set; }
    }
}
