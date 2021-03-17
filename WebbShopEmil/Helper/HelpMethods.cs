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
        
        /// <summary>
        /// Hjälp från david
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

        /// <summary>
        /// Hjälp från david
        /// </summary>
        /// <param name="bookId"></param>
        /// <param name="book"></param>
        /// <returns></returns>
        public static bool BookExists(int bookId, out Book book)
        {
            book = (from b in db.Books where b.Id == bookId select b).Include(b => b.Category).FirstOrDefault();

            return book != null;
        }

        /// <summary>
        /// Hjälp från david
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        public static bool CategoryExists(int categoryId, out BookCategory category)
        {
            category = (from c in db.BookCategories where c.Id == categoryId select c).FirstOrDefault();
            return category != null;
        }
    }
}
