using GearShop.Dtos.User;
using System;
using System.Collections.Generic;

namespace GearShop.Dtos.Post
{
    public class PostDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }

        public DateTime CreatedAt { get; set; }

        public required AuthorDto Author { get; set; }
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
        public bool IsLikedByCurrentUser { get; set; }

        public List<CommentDto> RecentComments { get; set; } = new();
    }


}
