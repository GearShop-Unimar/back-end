using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GearShop.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required string Content { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public bool IsRead { get; set; } = false;

        // Relacionamento com o remetente (Sender)
        [Required] // O ID é obrigatório
        public int SenderId { get; set; }
        [ForeignKey("SenderId")]
        public virtual User Sender { get; set; } = null!;

        // Relacionamento com a conversa
        [Required] // O ID é obrigatório
        public int ConversationId { get; set; }
        [ForeignKey("ConversationId")]
        public virtual Conversation Conversation { get; set; } = null!;
    }
}
