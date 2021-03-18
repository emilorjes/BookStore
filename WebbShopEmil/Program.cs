using System;
using WebbShopEmil.Database;
using WebbShopEmil.Models;
using static WebbShopEmil.Helper.HelpMethods;


namespace WebbShopEmil
{
    class Program
    {
        static void Main(string[] args)
        {
            Seeder.Seed();
            WebbShopAPI webbShop = new WebbShopAPI();
            var userId = webbShop.Login("Administrator", "CodicRulez");
            var books = webbShop.GetBook(2);
            //webbShop.Logout(userId);

            ForeachUsers(webbShop, userId);
            Console.WriteLine("-----------------------\n");
            
            ForeachCategories(webbShop);
            Console.WriteLine("-----------------------\n");
            
            ForeachCategoriesKeyword(webbShop, "h");
            Console.WriteLine("-----------------------\n");
            
            ForeachBooksKeyword(webbShop, "i");
            Console.WriteLine("-----------------------\n");
            
            ForeachAuthorsKeyword(webbShop, "o");
            Console.WriteLine("-----------------------\n");
            
            ForeachBooksByCategory(webbShop, 2);
            Console.ReadKey();
        }

       
    }
}
