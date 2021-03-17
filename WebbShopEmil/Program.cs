using System;
using WebbShopEmil.Database;
using WebbShopEmil.Models;

namespace WebbShopEmil
{
    class Program
    {
        static void Main(string[] args)
        {
            Seeder.Seed();
            WebbShopAPI shop = new WebbShopAPI();
            var userId = shop.Login("Administrator", "CodicRulez");
            var users = shop.FindUser(userId, "an");
            //Console.WriteLine(shop.DeleteCategory(userId, 2));


            foreach (var user in shop.ListUsers(userId))
            {
                Console.WriteLine(user.Name);
            }
            foreach (var category in shop.GetCategories())
            {
                Console.WriteLine($"Name: {category.Name}");
            }
            Console.ReadKey();
        }
    }
}
