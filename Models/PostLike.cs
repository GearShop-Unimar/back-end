using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace GearShop.Models
{
    public class PostLike
    {
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int PostId { get; set; }
        [ForeignKey("PostId")]
        public virtual Post? Post { get; set; }

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}