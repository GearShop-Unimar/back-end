using GearShop.Dtos.User;
using System;

namespace GearShop.Dtos.Post
{
    public class CommentDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public required AuthorDto Author { get; set; }
    }
}