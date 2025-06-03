using Domain.Enums;
using Infrastructure.Identity;

namespace Infrastructure.Data
{
    public class ChatEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public List<AppUser> Members { get; set; } = new List<AppUser>();

        public List<MessageEntity> Messages { get; set; } = new List<MessageEntity>();

        public ChatType Type { get; set; }
    }
}
