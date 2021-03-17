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
            Console.WriteLine(shop.DeleteCategory(userId, 2));



            //var list = shop.ListUsers(1);
            //foreach (var user in list)
            //{
            //    Console.WriteLine(user.Name);
            //}

            Console.ReadKey();
        }
    }
}
