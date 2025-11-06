using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using GearShop.Data;
using GearShop.Models;
using GearShop.Dtos.Message;
using GearShop.Dtos.User;

namespace GearShop.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MessagesController(AppDbContext context)
        {
            _context = context;
        }

        private int GetCurrentUserId()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int.TryParse(userIdString, out var userId);
            return userId;
        }

        private string ConvertProfilePictureToDataUri(byte[]? data, string? mimeType)
        {
            if (data == null || data.Length == 0 || string.IsNullOrEmpty(mimeType))
            {
                return "https://i.imgur.com/V4RclNb.png";
            }
            string base64String = Convert.ToBase64String(data);
            return $"data:{mimeType};base64,{base64String}";
        }


        // GET: api/messages
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ConversationSummaryDto>>> GetConversations()
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == 0) return Unauthorized();

            var conversations = await _context.Conversations
                .Include(c => c.ParticipantA)
                .Include(c => c.ParticipantB)
                .Include(c => c.Messages)
                .Where(c => c.ParticipantAId == currentUserId || c.ParticipantBId == currentUserId)
                .OrderByDescending(c => c.Messages.Any() ? c.Messages.Max(m => m.Timestamp) : DateTime.MinValue)
                .ToListAsync();

            var dtos = conversations.Select(c =>
            {
                var otherUser = c.ParticipantAId == currentUserId ? c.ParticipantB : c.ParticipantA;
                var lastMessage = c.Messages.OrderByDescending(m => m.Timestamp).FirstOrDefault();

                // Convertemos a imagem uma só vez
                string userAvatarUri = ConvertProfilePictureToDataUri(otherUser.ProfilePictureData, otherUser.ProfilePictureMimeType);

                return new ConversationSummaryDto
                {
                    Id = c.Id,
                    OtherUser = new UserDto
                    {
                        Id = otherUser.Id,
                        Name = otherUser.Name,
                        Role = otherUser.Role.ToString(),
                        PhoneNumber = otherUser.PhoneNumber,
                        Estado = otherUser.Estado,

                        // --- CORREÇÕES AQUI ---
                        // Adicionados os campos 'required' que faltavam
                        Email = otherUser.Email,
                        Cidade = otherUser.Cidade,

                        // Corrigindo a redundância (ver nota abaixo)
                        ProfilePicture = userAvatarUri,
                        Avatar = userAvatarUri
                    },
                    LastMessage = lastMessage?.Content ?? "Nenhuma mensagem",
                    LastTimestamp = lastMessage != null
                        ? ((DateTimeOffset)lastMessage.Timestamp).ToUnixTimeMilliseconds()
                        : 0,
                    Unread = c.Messages.Count(m => !m.IsRead && m.SenderId != currentUserId)
                };
            }).ToList();

            return Ok(dtos);
        }

        // GET: api/messages/conversation/{id}
        [HttpGet("conversation/{id}")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessages(int id)
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == 0) return Unauthorized();

            var conversation = await _context.Conversations
                .Include(c => c.Messages)
                .FirstOrDefaultAsync(c => c.Id == id && (c.ParticipantAId == currentUserId || c.ParticipantBId == currentUserId));

            if (conversation == null)
            {
                return NotFound("Conversa não encontrada ou não tem permissão.");
            }

            var dtos = conversation.Messages
                .OrderBy(m => m.Timestamp)
                .Select(m => new MessageDto
                {
                    Id = m.Id,
                    Text = m.Content,
                    SenderId = m.SenderId,
                    Timestamp = ((DateTimeOffset)m.Timestamp).ToUnixTimeMilliseconds()
                }).ToList();

            return Ok(dtos);
        }

        [HttpPost("conversation/{id}")]
        public async Task<ActionResult<MessageDto>> SendMessage(int id, [FromBody] CreateMessageDto createMessageDto)
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == 0) return Unauthorized();

            var conversation = await _context.Conversations
                .FirstOrDefaultAsync(c => c.Id == id && (c.ParticipantAId == currentUserId || c.ParticipantBId == currentUserId));

            if (conversation == null)
            {
                return Forbid("Não pode enviar mensagens para esta conversa.");
            }

            var message = new Message
            {
                Content = createMessageDto.Text,
                SenderId = currentUserId,
                ConversationId = id,
                Timestamp = DateTime.UtcNow,
                IsRead = false
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            var messageDto = new MessageDto
            {
                Id = message.Id,
                Text = message.Content,
                SenderId = message.SenderId,
                Timestamp = ((DateTimeOffset)message.Timestamp).ToUnixTimeMilliseconds()
            };

            return CreatedAtAction(nameof(GetMessages), new { id = conversation.Id }, messageDto);
        }
    }
}
