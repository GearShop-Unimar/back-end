using Microsoft.AspNetCore.Http;

namespace GearShop.Dtos.Post
{
    public class CreatePostDto
    {
        public string Content { get; set; } = string.Empty;

        public IFormFile? ImageFile { get; set; }
    }
}