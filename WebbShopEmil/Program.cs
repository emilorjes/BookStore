using System;
using WebbShopEmil.Database;
using static WebbShopEmil.Helper.HelpMethods;

namespace WebbShopEmil
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Seeder.Seed();
            WebbShopAPI webbShop = new WebbShopAPI();
            webbShop.Login("Emil", "Lime");
            //==========================================================================================================
            Line();
            var userId = webbShop.Login("TestCustomer", "Codic2021");
            Console.WriteLine($"User with Id {userId} is logged in");
            Line();
            //==========================================================================================================
            GreenTextWL("List all categories:");
            ForeachGetCategories(webbShop);
            Line();
            //==========================================================================================================
            GreenTextWL("List categories by keyword:");
            ForeachGetCategoriesKeyword(webbShop, "h");
            Line();
            //==========================================================================================================
            GreenTextWL("List books by category:");
            ForeachGetAvailableBooks(webbShop, 2);
            Line();
            //==========================================================================================================
            GreenTextWL("List books by keyword:");
            ForeachGetBooksKeyword(webbShop, "i");
            Line();
            //==========================================================================================================
            GreenTextWL("List authors by keyword:");
            ForeachGetAuthorsKeyword(webbShop, "a");
            Line();
            //==========================================================================================================
            GreenTextWL("Show a book with a specific id and show book amount");
            var book = webbShop.GetBook(4);
            Console.WriteLine($"Id:{book.Id}. Title: {book.Title}");
            Console.WriteLine($"Book amount: {book.Amount}");
            Line();
            //==========================================================================================================
            Console.WriteLine($"Buy book = { webbShop.BuyBook(userId, book.Id)}");
            Console.WriteLine($"Id:{book.Id}. Title: {book.Title}");
            Console.WriteLine($"Book amount: {book.Amount}");
            Line();
            //==========================================================================================================
            var adminId = webbShop.Login("Administrator", "CodicRulez");
            Console.WriteLine($"User with Id {adminId} is logged in");
            Line();
            //==========================================================================================================
            Console.WriteLine($"Add category = { webbShop.AddCategory(adminId, "Documentary")}");
            Line();
            //==========================================================================================================
            Console.WriteLine($"Add book to category = {webbShop.AddBookToCategory(adminId, book.Id, 3)}");
            Line();
            //==========================================================================================================
            Console.WriteLine($"Add new user = {webbShop.AddUser(adminId, "Emil", "Lime")}");
            Line();
            //==========================================================================================================
            GreenTextWL("List all users:");
            ForeachListUsers(webbShop, adminId);
            Line();
            //==========================================================================================================
            Console.ReadKey();
        }
    }
}