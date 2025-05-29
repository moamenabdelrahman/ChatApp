namespace Domain.Entities
{
    public class Chat
    {
        public int Id { get; set; }

        public ChatType ChatType { get; set; }

        public User Admin { get; set; }
    }

    public enum ChatType
    {
        Private = 0,
        Group = 1
    }
}
