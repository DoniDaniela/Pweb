using GameStore.Domain.Helper;
using GameStore.Domain.Identity;
using GameStore.Domain.Model;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameStore.Domain.Infrastructure
{
    public class IdentityDbInitializer : DropCreateDatabaseIfModelChanges<GameStoreDBContext>
    {
        protected override void Seed(GameStoreDBContext context)
        {
            PerformInitialSetup(context);
            base.Seed(context);
        }

        public void PerformInitialSetup(GameStoreDBContext context)
        {
            GetRoles().ForEach(c => context.Roles.Add(c));
            GetCategories().ForEach(c => context.Categories.Add(c));
            GetProducts().ForEach(c => context.Products.Add(c));
            context.SaveChanges();
            GameStorePasswordHasher hasher = new GameStorePasswordHasher();
            var user = new AppUser { UserName = "admin", Email = "admin@gameshop.com", PasswordHash = hasher.HashPassword("admin"), Membership = "Admin" };
            var role = context.Roles.Where(r => r.Name == "Admin").First();
            user.Roles.Add(new IdentityUserRole { RoleId = role.Id, UserId = user.Id });
            context.Users.Add(user);
            
            user = new AppUser { UserName = "user", Email = "user@gameshop.com", PasswordHash = hasher.HashPassword("User@2022"), Membership = "User" };
            role = context.Roles.Where(r => r.Name == "User").First();
            user.Roles.Add(new IdentityUserRole { RoleId = role.Id, UserId = user.Id });
            context.Users.Add(user);
            context.SaveChanges();
        }

        private static List<AppRole> GetRoles()
        {
            var roles = new List<AppRole> {
               new AppRole {Name="Admin", Description="Admin"},
               new AppRole {Name="User", Description="User"}
            };

            return roles;
        }

        private static List<Category> GetCategories()
        {
            var categories = new List<Category> {
               new Category {CategoryId=1, CategoryName="Applications"},
               new Category {CategoryId=2, CategoryName="Games"}
            };

            return categories;
        }

        private static List<Product> GetProducts()
        {
            var products = new List<Product> {
               new Product {ProductId=1, ProductName="Facebook", CategoryId = 1, Image="applications/facebook.png", Condition="New", UserId="Admin", OpenUrl="https://facebook.com"},
               new Product {ProductId=2, ProductName="Instagram", CategoryId = 1, Image="applications/instagram.png", Condition="New", UserId="Admin", OpenUrl="https://instagram.com"},
               new Product {ProductId=3, ProductName="Twitter", CategoryId = 1, Image="applications/twitter.png", Condition="New", UserId="Admin", OpenUrl="https://twitter.com"},
               new Product {ProductId=4, ProductName="Tiktok", CategoryId = 1, Image="applications/tiktok.png", Condition="New", UserId="Admin", OpenUrl="https://tiktok.com"},
               new Product {ProductId=5, ProductName="Youtube", CategoryId = 1, Image="applications/youtube.png", Condition="New", UserId="Admin", OpenUrl="https://youtube.com"},
               new Product {ProductId=6, ProductName="Pinterest", CategoryId = 1, Image="applications/pinterest.png", Condition="New", UserId="Admin", OpenUrl="https://pinterest.com"},
               new Product {ProductId=7, ProductName="FIFA 2016", CategoryId = 2, Image="games/ea_fifa.jpg", Condition="New", UserId="Admin"},
               new Product {ProductId=8, ProductName="Need for Speed", CategoryId = 2, Image="games/ea_nfs.jpg", Condition="New", UserId="Admin"},
               new Product {ProductId=9, ProductName="Call Of Duty", CategoryId = 2, Image="games/activision_cod.jpg", Condition="New", UserId="Admin"},
               new Product {ProductId=10, ProductName="Evolve", CategoryId = 2, Image="games/tti_evolve.jpg", Condition="New", UserId="Admin"},
            };

            return products;
        }
    }
}
