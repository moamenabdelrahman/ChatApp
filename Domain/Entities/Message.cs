namespace Domain.Entities
{
    public class Message
    {
        public int Id { get; set; }

        public User Sender { get; set; }

        public int ChatId { get; set; }

        public string Content { get; set; }

        public DateTime TimeStamp { get; set; }
    }
}
