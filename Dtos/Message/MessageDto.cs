namespace GearShop.Dtos.Message
{
    public class MessageDto
    {
        public int Id { get; set; }
        public required string Text { get; set; }
        public long Timestamp { get; set; }
        public required int SenderId { get; set; }
    }
}
