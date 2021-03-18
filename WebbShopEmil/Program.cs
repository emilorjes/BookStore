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
            
            var userId = webbShop.Login("TestCustomer", "Codic2021");
            Console.WriteLine($"User with Id {userId} is logged in");
            Console.WriteLine("-----------------------\n");

            Console.WriteLine("List all categories:");
            ForeachCategories(webbShop);
            Console.WriteLine("-----------------------\n");

            Console.WriteLine("Print list with all books by category:");
            ForeachBooksByCategory(webbShop, 2);
            Console.WriteLine("-----------------------\n");

            Console.WriteLine("Show a book with a specific id and show book amount");
            var book = webbShop.GetBook(2);
            Console.WriteLine($"Book amount: {book.Amount} ID: {book.Id}");
            Console.WriteLine("-----------------------\n");

            Console.WriteLine("Köper bok om boken finns.");
            webbShop.BuyBook(userId, book.Id);
            Console.WriteLine($"Book amount: {book.Amount} ID: {book.Id}");
            Console.WriteLine("-----------------------\n");
            
            Console.WriteLine("-----------------------\n");
            Console.WriteLine("-----------------------\n");
            Console.WriteLine("-----------------------\n");

            var adminId = webbShop.Login("Administrator", "CodicRulez");
            Console.WriteLine($"User with Id {adminId} is logged in");
            Console.WriteLine("-----------------------\n");

            Console.WriteLine("Add category.");
            webbShop.AddCategory(adminId, "Documentary");
            Console.WriteLine("-----------------------\n");

            Console.WriteLine("Move book to category.");
            webbShop.AddBookToCategory(adminId, book.Id, 2);
            Console.WriteLine("-----------------------\n");

            Console.WriteLine("Add new user");
            var newUser = webbShop.AddUser(adminId, null, null);
            Console.WriteLine(newUser);
            Console.ReadKey();


























        }


    }
}
