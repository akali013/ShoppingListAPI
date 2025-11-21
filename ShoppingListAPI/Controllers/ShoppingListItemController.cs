using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingListAPI.Authorization;
using ShoppingListAPI.Data;
using ShoppingListAPI.Models;
using ShoppingListAPI.Services;

namespace ShoppingListAPI.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class ShoppingListItemController : Controller
    {
        private IShoppingListItemService _shoppingService;
        public ShoppingListItemController(IShoppingListItemService shoppingService)
        { 
            _shoppingService = shoppingService;
        }

        [HttpGet]
        public ActionResult<List<ShoppingListItemResponse>> GetListItems()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var userItems = _shoppingService.GetUserItems(refreshToken);
            return Ok(userItems);
        }

        [HttpPost]
        public ActionResult<ShoppingListItem> AddListItem([FromBody] ShoppingListItemRequest request)
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var newItem = _shoppingService.AddUserItem(refreshToken, request);
            return Ok(newItem);
        }

        [HttpPut]
        public ActionResult<ShoppingListItem> UpdateListItem([FromBody] ShoppingListItemRequest request)
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var updatedItem = _shoppingService.UpdateUserItem(refreshToken, request);
            return Ok(updatedItem);
        }

        [HttpDelete]
        public IActionResult DeleteItem([FromQuery] ShoppingListItemRequest model)
        {
            var requestToken = Request.Cookies["refreshToken"];
            _shoppingService.DeleteUserItem(requestToken, model);
            return Ok(new {message = "Item deleted!"});
        }
    }
}
