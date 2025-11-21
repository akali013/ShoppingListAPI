using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShoppingListAPI.Authorization;
using ShoppingListAPI.Data;
using ShoppingListAPI.Models;
using ShoppingListAPI.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingListAPI.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class ItemsController : Controller
    {
        private IItemService _itemService;

        public ItemsController(IItemService itemService)
        {
            _itemService = itemService;
        }

        [HttpGet]
        public ActionResult<List<Item>> GetAllItems()
        {
            var items = _itemService.GetAllItems();
            return Ok(items);
        }

        [HttpGet("{id}")]
        public ActionResult<Item> GetItem(Guid id)
        {
            var item = _itemService.GetItem(id);
            return Ok(item);
        }

        [HttpGet("search/{searchTerm}")]
        public ActionResult<List<Item>> SearchItems(string searchTerm)
        {
            var searchedItems = _itemService.SearchItems(searchTerm);
            return Ok(searchedItems);
        }


        [HttpPost]
        public ActionResult<Item> CreateItem([FromBody] ItemRequest dto)
        {
            var newItem = _itemService.CreateItem(dto);
            return CreatedAtAction(nameof(GetAllItems), new { id = newItem.Id }, newItem);
        }
    }
}
