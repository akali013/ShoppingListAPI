using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingListAPI.Data;
using ShoppingListAPI.Helpers;
using ShoppingListAPI.Models;

namespace ShoppingListAPI.Services
{
    public interface IItemService
    {
        List<Item> GetAllItems();
        Item GetItem(Guid id);
        List<Item> SearchItems(string searchTerm);
        Item CreateItem(ItemRequest dto);
    }

    public class ItemService : IItemService
    {
        private ShoppingListAPIContext _context;

        public ItemService(ShoppingListAPIContext context)
        {
            _context = context;
        }

        public List<Item> GetAllItems()
        {
            return _context.Items.ToList();
        }

        public Item GetItem(Guid id)
        {
            var item = _context.Items.Single(item => item.Id == id);
            
            if (item == null)
            {
                throw new KeyNotFoundException("Item not found!");
            }

            return item;
        }

        public List<Item> SearchItems(string searchTerm)
        {
            if (_context.Items is null)
            {
                throw new AppException("Entity 'Item' does not exist.");
            }

            var items = from item in _context.Items
                        select item;

            if (!String.IsNullOrEmpty(searchTerm))
            {
                items = items.Where(item => item.Name.ToUpper().Contains(searchTerm.ToUpper()));
            }

            return items.ToList();
        }

        public Item CreateItem([FromBody] ItemRequest dto)
        {
            var existingItem = _context.Items.FirstOrDefault(item => item.Name == dto.Name);
            if (existingItem is not null)
            {
                throw new AppException("Item already exists.");
            }

            if (dto is not null)
            {
                var item = new Item();
                item.Id = Guid.NewGuid();
                item.Name = dto.Name;
                _context.Items.Add(item);
                _context.SaveChanges();
                return item;
            }
            throw new AppException("Item is required.");
        }
    }
}
