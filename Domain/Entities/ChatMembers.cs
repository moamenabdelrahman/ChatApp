namespace Domain.Entities
{
    public class ChatMembers
    {
        public int Id { get; set; }

        public Chat Chat { get; set; }

        public User Member { get; set; }
    }
}
