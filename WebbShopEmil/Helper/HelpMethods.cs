using System;
using System.Collections.Generic;
using System.Text;
using WebbShopEmil.Database;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using WebbShopEmil.Models;

namespace WebbShopEmil.Helper
{
    public static class HelpMethods
    {
        public static WebbShopContext db = new WebbShopContext();
        private const int MaxSessionTime = -15;
        
        // Lånat denna metoden av David Ström,
        // han har förklarat för mig hur denna metod fungerar och varför vi använder den.
        /// <summary>
        /// Checks if user is an admin,
        /// and if user is logged in.
        /// </summary>
        /// <param name="adminId"></param>
        /// <returns></returns>
        public static bool UserIsAdminAndLoggedIn(int adminId)
        {
            var adminUser = (from u in db.Users
                             where u.Id == adminId
                             && u.IsActive == true
                             && u.IsAdmin == true
                             && u.SessionTimer > DateTime.Now.AddMinutes(MaxSessionTime)
                             select u).FirstOrDefault();

            return adminUser != null;
        }

        // Lånat denna metoden av David Ström,
        // han har förklarat för mig hur denna metod fungerar och varför vi använder den.
        /// <summary>
        /// Checks if book exists.
        /// </summary>
        /// <param name="bookId"></param>
        /// <param name="book"></param>
        /// <returns></returns>
        public static bool BookExists(int bookId, out Book book)
        {
            book = (from b in db.Books where b.Id == bookId select b).Include(b => b.Category).FirstOrDefault();

            return book != null;
        }

        // Lånat denna metoden av David Ström,
        // han har förklarat för mig hur denna metod fungerar och varför vi använder den.
        /// <summary>
        /// Checks if category exists.
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        public static bool CategoryExists(int categoryId, out BookCategory category)
        {
            category = (from c in db.BookCategories where c.Id == categoryId select c).FirstOrDefault();
            return category != null;
        }

        // Lånat denna metoden av David Ström,
        // han har förklarat för mig hur denna metod fungerar och varför vi använder den.
        /// <summary>
        /// Checks if user exists.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public static bool UserExists(int userId, out User user)
        {
            user = (from u in db.Users where u.Id == userId select u).FirstOrDefault();
            return user != null;
        }

        public static void ForeachBooksByCategory(WebbShopAPI webbShop, int categoryId)
        {
            foreach (var book in webbShop.GetAvailableBooks(categoryId))
            {
                Console.WriteLine($"Book title: {book.Title}");
            }
        }

        public static void ForeachAuthorsKeyword(WebbShopAPI webbShop, string keyword)
        {
            foreach (var book in webbShop.GetAuthors(keyword))
            {
                Console.WriteLine($"Book author: {book.Author}");
            }
        }

        public static void ForeachBooksKeyword(WebbShopAPI webbShop, string keyword)
        {
            foreach (var book in webbShop.GetBooks(keyword))
            {
                Console.WriteLine($"Book title: {book.Title}");
            }
        }

        public static void ForeachCategoriesKeyword(WebbShopAPI webbShop, string keyword)
        {
            foreach (var category in webbShop.GetCategories(keyword))
            {
                Console.WriteLine($"Category name: {category.Name}");
            }
        }

        public static void ForeachCategories(WebbShopAPI webbShop)
        {
            foreach (var category in webbShop.GetCategories())
            {
                Console.WriteLine($"Category name: {category.Name}");
            }
        }

        public static void ForeachUsers(WebbShopAPI webbShop, int userId)
        {
            foreach (var user in webbShop.ListUsers(userId))
            {
                Console.WriteLine($"User name: {user.Name}");
            }
        }
    }
}
