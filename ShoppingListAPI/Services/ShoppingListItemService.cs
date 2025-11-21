using ShoppingListAPI.Data;
using ShoppingListAPI.Helpers;
using ShoppingListAPI.Models;

namespace ShoppingListAPI.Services
{
    public interface IShoppingListItemService
    {
        public List<ShoppingListItemResponse> GetUserItems(string refreshToken);
        public ShoppingListItem AddUserItem(string refreshToken, ShoppingListItemRequest model);
        public ShoppingListItem UpdateUserItem(string refreshToken, ShoppingListItemRequest model);
        public void DeleteUserItem(string refreshToken, ShoppingListItemRequest model);
    }

    public class ShoppingListItemService : IShoppingListItemService
    {
        private ShoppingListAPIContext _context;

        public ShoppingListItemService(ShoppingListAPIContext context)
        {
            _context = context;
        }

        // Returns a list of items with their name, quantity, and checked status.
        public List<ShoppingListItemResponse> GetUserItems(string refreshToken)
        {
            User user = getUserByRefreshToken(refreshToken);
            List<ShoppingListItem> userItems = _context.ShoppingListItems.Where(item => item.UserId == user.Id).ToList<ShoppingListItem>();
            List<ShoppingListItemResponse> displayedItems = (
                from listItem in userItems
                join item in _context.Items on listItem.ItemId equals item.Id
                select new ShoppingListItemResponse(item.Name, listItem.Quantity, listItem.IsChecked)
            ).ToList<ShoppingListItemResponse>();

            return displayedItems;
        }

        public ShoppingListItem AddUserItem(string refreshToken, ShoppingListItemRequest model)
        {
            User user = getUserByRefreshToken(refreshToken);
            Item item = getItemByName(model.Name);
            var newListItem = new ShoppingListItem();
            newListItem.UserId = user.Id;
            newListItem.ItemId = item.Id;
            newListItem.Quantity = model.Quantity;
            newListItem.IsChecked = false;

            _context.ShoppingListItems.Add(newListItem);
            _context.SaveChanges();

            return newListItem;
        }

        public ShoppingListItem UpdateUserItem(string refreshToken, ShoppingListItemRequest model)
        {
            User user = getUserByRefreshToken(refreshToken);
            Item item = getItemByName(model.Name);
            ShoppingListItem listItem = _context.ShoppingListItems.Single(i => i.UserId == user.Id && i.ItemId == item.Id);
            listItem.Quantity = model.Quantity;
            listItem.IsChecked = model.IsChecked;

            _context.ShoppingListItems.Update(listItem);
            _context.SaveChanges();

            return listItem;
        }

        public void DeleteUserItem(string refreshToken, ShoppingListItemRequest model)
        {
            User user = getUserByRefreshToken(refreshToken);
            Item item = getItemByName(model.Name);
            ShoppingListItem deletedItem = _context.ShoppingListItems.Single(i => i.UserId == user.Id && i.ItemId == item.Id);

            _context.Remove(deletedItem);
            _context.SaveChanges();
        }


        private User getUserByRefreshToken(string token)
        {
            var user = _context.Users.SingleOrDefault<User>(u => u.RefreshTokens.Any(t => t.Token == token));

            if (user == null)
            {
                throw new AppException("Invalid token");
            }

            return user;
        }

        private Item getItemByName(string name)
        {
            var item = _context.Items.Single(item => item.Name == name);
            return item;
        }
    }
}
