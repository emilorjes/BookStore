using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using WebbShopEmil.Database;
using WebbShopEmil.Models;
using static WebbShopEmil.Helper.HelpMethods;

namespace WebbShopEmil
{
    class WebbShopAPI
    {
        private int maxSessionTime = -15;
        private int zero = 0;
        /// <summary>
        /// 
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
        /// 
        /// </summary>
        /// <param name="adminId"></param>
        /// <param name="name"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool AddUser(int adminId, string name, string password)
        {
            if (UserIsAdminAndLoggedIn(adminId))
            {
                var user = (from u in db.Users where u.Name == name && u.Password == password select u).FirstOrDefault();
                if (user == null && password != string.Empty)
                {
                    db.Users.Add(new User { Name = name, Password = password, IsAdmin = false });
                    db.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Buy book.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="bookId"></param>
        /// <returns></returns>
        public bool BuyBook(int userId, int bookId)
        {
            var user = (from u in db.Users where u.Id == userId && u.IsActive && u.SessionTimer > DateTime.Now.AddMinutes(maxSessionTime) select u).FirstOrDefault();

            if (user != null && BookExists(bookId, out Book book) && book.Amount > zero)
            {
                if (book != null)
                {
                    db.SoldBooks.Add(new SoldBook
                    {
                        Title = book.Title,
                        Author = book.Author,
                        CategoryId = book.Category, // Ä-----------------------------------------------------------------ndra
                        Price = book.Price,
                        PurchasedDate = DateTime.Now,
                        User = user
                    });
                    book.Amount --;
                    db.Books.Update(book);
                    db.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        public bool DeleteBook(int adminId, int bookId)
        {
           
            if (UserIsAdminAndLoggedIn(adminId) 
                && BookExists(bookId, out Book book))
            {
                book.Amount--;
                if (book.Amount > zero)
                {
                    db.Books.Update(book);
                    db.SaveChanges();
                }
                else if (book.Amount <= zero)
                {
                    db.Books.Remove(book);
                    db.SaveChanges();
                    return true;
                }

            }
            return false;
        }

        public bool DeleteCategory(int adminId, int categoryId)
        {
            if(UserIsAdminAndLoggedIn(adminId ) && CategoryExists(categoryId, out var category))
            {
                //try
                //{
                //    db.BookCategories.Remove(category);
                //    db.SaveChanges();
                //    return true;
                //}
                //catch (Exception)
                //{
                //    return false;
                //}

                var books = (from b in db.Books where b.Category == category select b);

                if(books.Count() == 0)
                {
                    db.BookCategories.Remove(category);
                    db.SaveChanges();
                    return true;
                }
              
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="adminId"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public IEnumerable<User> FindUser(int adminId, string keyword)
        {
            var user = (from u in db.Users where u.Id == adminId && u.SessionTimer > DateTime.Now.AddMinutes(maxSessionTime) select u).FirstOrDefault();
            if (user.IsAdmin == true)
            {
                return from u in db.Users where u.Name.Contains(keyword) select u;
            }
            return new User[0];
        }

        /// <summary>
        /// Get a list of authors that contains a specific keyword.
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public IEnumerable<Book> GetAuthors(string keyword)
        {
            return from b in db.Books where b.Author.Contains(keyword) select b;
        }

        /// <summary>
        /// Get a list of books by categoryId.
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public IEnumerable<Book> GetAvailableBooks(int categoryId)
        {
            return from b in db.Books where b.Category.Id == categoryId && b.Amount > zero select b;
        }

        /// <summary>
        /// Get a specific book by bookId.
        /// </summary>
        /// <param name="bookId"></param>
        /// <returns></returns>
        public IEnumerable<Book> GetBook(int bookId)
        {
            return from b in db.Books where b.Id == bookId select b;
        }

        /// <summary>
        /// Get a list of books that contains a specific keyword.
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public IEnumerable<Book> GetBooks(string keyword)
        {
            return from b in db.Books where b.Title.Contains(keyword) select b;
        }

        /// <summary>
        /// Get a list of all categories, order by name.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<BookCategory> GetCategories()
        {
            return from c in db.BookCategories orderby c.Name select c;
        }

        /// <summary>
        /// Get a list of categories that contains a specific keyword, order by name.
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public IEnumerable<BookCategory> GetCategories(string keyword)
        {
            return from c in db.BookCategories where c.Name.Contains(keyword) orderby c.Name select c;
        }

        /// <summary>
        /// Get a list of category that has a specific categoryId.
        /// </summary>
        /// <param name="categoryID"></param>
        /// <returns></returns>
        public IEnumerable<Book> GetCategory(int categoryId)
        {
            return from c in db.Books where c.Category.Id == categoryId select c;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="adminId"></param>
        /// <returns></returns>
        public IEnumerable<User> ListUsers(int adminId)
        {
            var user = (from u in db.Users where u.Id == adminId && u.SessionTimer > DateTime.Now.AddMinutes(maxSessionTime) select u).FirstOrDefault();
            if (user.IsAdmin == true)
            {
                return from u in db.Users orderby u.Name select u;
            }
            return new User[0];
        }

        /// <summary>
        /// 
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
        /// Logout user if DateTime is not updated in 15 min (maxSessionTime).
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool Logout(int userId)
        {
            var user = (from u in db.Users where u.Id == userId select u).FirstOrDefault();
            if (user != null && user.SessionTimer > DateTime.Now.AddMinutes(maxSessionTime))
            {
                user.SessionTimer = DateTime.MinValue;
                db.Users.Update(user);
                db.SaveChanges();
                return true;
            }
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string Ping(int userId)
        {
            var user = from u in db.Users where u.Id == userId && u.SessionTimer > DateTime.Now.AddMinutes(maxSessionTime) select u;
            if (user != null)
            {

                return "Pong";
            }
            return string.Empty;
        }

        /// <summary>
        /// Register new user if the user dont alredy exists. 
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
                    db.Users.Add(new User { Name = name, Password = password, IsAdmin = false });
                    db.SaveChanges();
                    return true;
                }
            }
            return false;
        }
        public void SetAmount(int adminId, int bookId)
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="adminId"></param>
        /// <param name="id"></param>
        /// <param name="title"></param>
        /// <param name="author"></param>
        /// <param name="price"></param>
        /// <returns></returns>
        public bool UpdateBook(int adminId, int id, string title, string author, int price)
        {
            var user = (from u in db.Users where u.Id == adminId && u.SessionTimer > DateTime.Now.AddMinutes(maxSessionTime) select u).FirstOrDefault();
            if (user.IsAdmin == true)
            {
                var book = (from b in db.Books select b).FirstOrDefault();
                if (book.Id == id)
                {
                    book.Title = title;
                    book.Author = author;
                    book.Price = price;
                    db.Books.Update(book);
                    db.SaveChanges();
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="adminId"></param>
        /// <param name="categoryId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool UpdateCategory(int adminId, int categoryId, string name)
        {
            var user = (from u in db.Users where u.Id == adminId && u.SessionTimer > DateTime.Now.AddMinutes(maxSessionTime) select u).FirstOrDefault();
            if (user.IsAdmin == true)
            {
                var category = (from c in db.BookCategories select c).FirstOrDefault();
                if (category.Id == categoryId)
                {
                    category.Name = name;
                    db.BookCategories.Update(category);
                    db.SaveChanges();
                    return true;
                }
            }
            return false;
        }




    }
}
