using Infrastructure.Identity;

namespace Infrastructure.Data
{
    public class MessageEntity
    {
        public int Id { get; set; }

        public int ChatId { get; set; }

        public int SenderId { get; set; }
        public AppUser Sender { get; set; }

        public string Content { get; set; }

        public DateTime TimeStamp { get; set; }
    }
}
