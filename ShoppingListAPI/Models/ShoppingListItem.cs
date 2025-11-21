using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ShoppingListAPI.Models
{
    public class ShoppingListItem
    {
        [Required]
        public Guid ItemId { get; set; }
        [Required]
        public Guid UserId { get; set; }
        public int Quantity { get; set; } = 1;
        public Boolean IsChecked { get; set; } = false;
    }
}
