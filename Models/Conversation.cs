using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GearShop.Models
{
    public class Conversation
    {
        [Key]
        public int Id { get; set; }

        public int ParticipantAId { get; set; }
        [ForeignKey("ParticipantAId")]
        public virtual required User ParticipantA { get; set; }

        public int ParticipantBId { get; set; }
        [ForeignKey("ParticipantBId")]
        public virtual required User ParticipantB { get; set; }

        public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
