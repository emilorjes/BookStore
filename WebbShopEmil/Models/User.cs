using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WebbShopEmil.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }// required key
        public string Password { get; set; }// required key
        public DateTime LastLogin { get; set; }
        public DateTime SessionTimer { get; set; }
        public bool IsActive { get; set; }
        public bool IsAdmin { get; set; }
    }
}
