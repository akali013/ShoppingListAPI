using System.ComponentModel.DataAnnotations;

namespace ShoppingListAPI.Models
{
    public class Item
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
