using GearShop.Dtos.User;

namespace GearShop.Dtos.Message
{
    public class ConversationSummaryDto
    {
        public int Id { get; set; }
        public required UserDto OtherUser { get; set; }
        public required string LastMessage { get; set; }
        public long LastTimestamp { get; set; }
        public int Unread { get; set; }
    }
}
