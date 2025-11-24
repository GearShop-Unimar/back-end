using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GearShop.Models
{
    public class Cart
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }

        public User? User { get; set; }

        public List<CartItem> Items { get; set; } = new List<CartItem>();

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
