using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ShoppingListAPI.Models;

namespace ShoppingListAPI.Data
{
    public class ShoppingListAPIContext : DbContext
    {
        public ShoppingListAPIContext (DbContextOptions<ShoppingListAPIContext> options)
            : base(options)
        {
        }

        // Register ShoppingListItem with a composite primary key
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ShoppingListItem>().HasKey(item => new { item.ItemId, item.UserId });
        }

        public DbSet<Item> Items { get; set; } = default!;
        public DbSet<User> Users { get; set; } = default!;
        public DbSet<ShoppingListItem> ShoppingListItems { get; set; } = default!;

        // RefreshToken is an Owned entity under Users, so it doesn't need a DbSet
    }
}
