using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using WebbShopEmil.Database;
using WebbShopEmil.Models;
using static WebbShopEmil.Helper.HelpMethods;

namespace WebbShopEmil
{
    public class WebbShopAPI
    {
        private const int MaxSessionTime = -15;
        private const int Zero = 0;

        /// <summary>
        /// Admin:
        /// If book alredy exists add amount to existing book.
        /// If book does not exists, 
        /// add book.
        /// </summary>
        /// <param name="adminId"></param>
        /// <param name="title"></param>
        /// <param name="author"></param>
        /// <param name="price"></param>
        /// <param name="amount"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool AddBook(int adminId,
            string title,
            string author, int price,
            int amount,
            int id = default)
        {
            if (UserIsAdminAndLoggedIn(adminId))
            {
                var book = (from b in db.Books where b.Id == id select b).FirstOrDefault();
                if (book != null)
                {
                    book.Amount += amount;
                    db.Update(book);
                }
                else
                {
                    db.Books.Add(new Book
                    {
                        Title = title,
                        Author = author,
                        Price = price,
                        Amount = amount
                    });
                }
                db.SaveChanges();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Admin:
        /// If book exist,
        /// and if category exists,
        /// add book to category.
        /// </summary>
        /// <param name="adminId"></param>
        /// <param name="bookId"></param>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public bool AddBookToCategory(int adminId, int bookId, int categoryId)
        {
            if (UserIsAdminAndLoggedIn(adminId))
            {

                if (BookExists(bookId, out var book))
                {
                    if (CategoryExists(categoryId, out var category))
                    {
                        book.Category = category;
                        db.Books.Update(book);
                        db.SaveChanges();
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Admin:
        /// If category does not alredy exists,
        /// add category.
        /// </summary>
        /// <param name="adminId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool AddCategory(int adminId, string name)
        {
            if (UserIsAdminAndLoggedIn(adminId))
            {
                var category = (from c in db.BookCategories where c.Name == name select c).FirstOrDefault();
                if (category == null)
                {
                    db.BookCategories.Add(new BookCategory { Name = name });
                    db.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Admin:
        /// If user does not alredy exists, 
        /// and if password is not empty,
        /// add user.
        /// </summary>
        /// <param name="adminId"></param>
        /// <param name="name"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool AddUser(int adminId, string name, string password)
        {
            if (UserIsAdminAndLoggedIn(adminId))
            {
                try
                {
                    var user = (from u in db.Users where u.Name == name && u.Password == password select u).FirstOrDefault();
                    if (user == null && password != string.Empty)
                    {
                        db.Users.Add(new User { Name = name, Password = password, IsAdmin = false, IsActive = true });
                        db.SaveChanges();
                        return true;
                    }
                }
                catch (Exception)
                {

                }
            }
            return false;
        }

        /// <summary>
        /// Admin / User:
        /// If book exists,
        /// buy book.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="bookId"></param>
        /// <returns></returns>
        public bool BuyBook(int userId, int bookId)
        {
            var user = (from u in db.Users where u.Id == userId && u.IsActive && u.SessionTimer > DateTime.Now.AddMinutes(MaxSessionTime) select u).FirstOrDefault();
            if (user != null && BookExists(bookId, out Book book) && book.Amount > Zero)
            {
                if (book != null)
                {
                    db.SoldBooks.Add(new SoldBook
                    {
                        Title = book.Title,
                        Author = book.Author,
                        CategoryId = book.Category,
                        Price = book.Price,
                        PurchasedDate = DateTime.Now,
                        User = user
                    });
                    book.Amount--;
                    db.Books.Update(book);
                    db.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Admin:
        /// If book amount is more than zero subtract amount by one.
        /// If book exists and amount is less or equal to zero,
        /// delete book.
        /// 
        /// </summary>
        /// <param name="adminId"></param>
        /// <param name="bookId"></param>
        /// <returns></returns>
        public bool DeleteBook(int adminId, int bookId)
        {

            if (UserIsAdminAndLoggedIn(adminId) && BookExists(bookId, out Book book))
            {
                book.Amount--;
                if (book.Amount > Zero)
                {
                    db.Books.Update(book);
                    db.SaveChanges();
                }
                else if (book.Amount <= Zero)
                {
                    db.Books.Remove(book);
                    db.SaveChanges();
                    return true;
                }

            }
            return false;
        }

        /// <summary>
        /// Admin:
        /// If category exists, 
        /// and book count is equal to zero,
        /// delete category.
        /// </summary>
        /// <param name="adminId"></param>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public bool DeleteCategory(int adminId, int categoryId)
        {
            if (UserIsAdminAndLoggedIn(adminId) && CategoryExists(categoryId, out var category))
            {
                var books = (from b in db.Books where b.Category == category select b);

                if (books.Count() == Zero)
                {
                    db.BookCategories.Remove(category);
                    db.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Admin:
        /// Find users that contains a keyword that match with user name.
        /// </summary>
        /// <param name="adminId"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public List<User> FindUsers(int adminId, string keyword)
        {
            var users = new List<User>();
            if (UserIsAdminAndLoggedIn(adminId))
            {
                users = (from u in db.Users where u.Name.Contains(keyword) orderby u.Name select u).ToList();
            }
            return users;
        }

        /// <summary>
        /// Admin / User:
        /// Get authors that contains a keyword that match with author name.
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public List<Book> GetAuthors(string keyword)
        {
            return (from b in db.Books where b.Author.Contains(keyword) orderby b.Title select b).ToList();
        }

        /// <summary>
        /// Admin / User:
        /// Get available books by category.
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public List<Book> GetAvailableBooks(int categoryId)
        {
            return (from b in db.Books where b.Category.Id == categoryId && b.Amount > Zero orderby b.Title select b).ToList();
        }

        /// <summary>
        ///  Admin / User:
        /// Get a specific book by book Id.
        /// </summary>
        /// <param name="bookId"></param>
        /// <returns></returns>
        public Book GetBook(int bookId)
        {
            return (from b in db.Books where b.Id == bookId select b).Include(b => b.Category).FirstOrDefault();
        }

        /// <summary>
        /// Admin / User:
        /// Get books that contains a keyword that match with title name.
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public List<Book> GetBooks(string keyword)
        {
            return (from b in db.Books where b.Title.Contains(keyword) orderby b.Title select b).ToList();
        }

        /// <summary>
        /// Admin / User:
        /// Get all categories.
        /// </summary>
        /// <returns></returns>
        public List<BookCategory> GetCategories()
        {
            return (from c in db.BookCategories orderby c.Name select c).ToList();
        }

        /// <summary>
        /// Admin / User:
        /// Get categories that contains a keyword that match with category name.
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public List<BookCategory> GetCategories(string keyword)
        {
            return (from c in db.BookCategories where c.Name.Contains(keyword) orderby c.Name select c).ToList();
        }

        /// <summary>
        /// Admin / User:
        /// Get category that has a specific category Id.
        /// </summary>
        /// <param name="categoryID"></param>
        /// <returns></returns>
        public List<Book> GetCategory(int categoryId)
        {
            return (from c in db.Books where c.Category.Id == categoryId orderby c.Title select c).ToList();
        }

        /// <summary>
        /// Admin:
        /// List all users:
        /// </summary>
        /// <param name="adminId"></param>
        /// <returns></returns>
        public List<User> ListUsers(int adminId)
        {
            var users = new List<User>();
            if (UserIsAdminAndLoggedIn(adminId))
            {
                users = (from u in db.Users orderby u.Name select u).ToList();
            }
            return users;
        }

        /// <summary>
        /// Admin / User: 
        /// Login user or admin.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public int Login(string username, string password)
        {
            var user = (from u in db.Users where u.Name == username && u.Password == password && u.IsActive select u).FirstOrDefault();
            if (user != null)
            {
                user.SessionTimer = DateTime.Now;
                user.LastLogin = DateTime.Now;
                db.Users.Update(user);
                db.SaveChanges();
                return user.Id;
            }
            return 0;
        }

        /// <summary>
        /// Admin / User: 
        /// Logout user or admin.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public void Logout(int userId)
        {
            var user = (from u in db.Users where u.Id == userId select u).FirstOrDefault();
            if (user != null)
            {
                user.SessionTimer = DateTime.MinValue;
                db.Users.Update(user);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Admin / User:
        /// Ping.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string Ping(int userId)
        {
            if (UserExists(userId, out User user) && user.SessionTimer > DateTime.Now.AddMinutes(MaxSessionTime))
            {
                user.SessionTimer = DateTime.Now;
                db.Users.Update(user);
                db.SaveChanges();
                return "Pong";
            }
            return string.Empty;
        }

        /// <summary>
        /// User:
        /// If the user dont alredy exists,
        /// register new user.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="password"></param>
        /// <param name="passwordVerify"></param>
        /// <returns></returns>
        public bool Register(string name, string password, string passwordVerify)
        {
            if (password == passwordVerify)
            {
                var user = (from u in db.Users where u.Name == name && u.Password == password select u).FirstOrDefault();
                if (user == null)
                {
                    db.Users.Add(new User { Name = name, Password = password, IsAdmin = false, IsActive = true }); ;
                    db.Users.Update(user);
                    db.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Admin:
        /// If book exists,
        /// sets the book amount.
        /// </summary>
        /// <param name="adminId"></param>
        /// <param name="bookId"></param>
        /// <param name="amount"></param>
        public void SetAmount(int adminId, int bookId, int amount)
        {
            if (UserIsAdminAndLoggedIn(adminId) && BookExists(bookId, out Book book))
            {
                book.Amount = amount;
                db.Books.Update(book);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Admin:
        /// If book exists,
        /// update book.
        /// </summary>
        /// <param name="adminId"></param>
        /// <param name="id"></param>
        /// <param name="title"></param>
        /// <param name="author"></param>
        /// <param name="price"></param>
        /// <returns></returns>
        public bool UpdateBook(int adminId, int bookId, string title, string author, int price)
        {
            if (UserIsAdminAndLoggedIn(adminId) && BookExists(bookId, out Book book))
            {
                book.Title = title;
                book.Author = author;
                book.Price = price;
                db.Books.Update(book);
                db.SaveChanges();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Admin:
        /// If category exists
        /// update category.
        /// </summary>
        /// <param name="adminId"></param>
        /// <param name="categoryId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool UpdateCategory(int adminId, int categoryId, string name)
        {
            if (UserIsAdminAndLoggedIn(adminId) && CategoryExists(categoryId, out BookCategory category))
            {
                category.Name = name;
                db.BookCategories.Update(category);
                db.SaveChanges();
                return true;
            }
            return false;
        }




    }
}
