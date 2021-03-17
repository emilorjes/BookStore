using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WebbShopEmil.Models
{
    public class SoldBook
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public BookCategory CategoryId { get; set; }// catid till cat
        public int Price { get; set; }
        public DateTime PurchasedDate { get; set; }
        public User User { get; set; }
    }
}
