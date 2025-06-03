using Domain.Enums;

namespace Domain.Entities
{
    public class Chat
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public List<Message> Messages { get; set; }

        public List<User> Members { get; set; }

        public ChatType Type { get; set; }
    }
}