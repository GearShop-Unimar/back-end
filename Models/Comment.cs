using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace GearShop.Models
{
    public class Comment
    {
        public int Id { get; set; }

        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int PostId { get; set; }
        [ForeignKey("PostId")]
        public virtual Post? Post { get; set; }

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User? Author { get; set; }
    }
}