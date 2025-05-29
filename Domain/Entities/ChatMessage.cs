namespace Domain.Entities
{
    public class ChatMessage
    {
        public int Id { get; set; }

        public Chat Chat { get; set; }

        public User Sender { get; set; }

        public string Content { get; set; }

        public DateTime TimeStamp { get; set; }
    }
}
