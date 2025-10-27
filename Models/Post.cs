using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace GearShop.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public byte[]? ImageData { get; set; }
        public string? ImageMimeType { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User? Author { get; set; }
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public virtual ICollection<PostLike> Likes { get; set; } = new List<PostLike>();
    }
}