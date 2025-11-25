using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace GearShop.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }  // <- USAR URL

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int UserId { get; set; }
        public virtual User? Author { get; set; }

        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public virtual ICollection<PostLike> Likes { get; set; } = new List<PostLike>();
    }

}
