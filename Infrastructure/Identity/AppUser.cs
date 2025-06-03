using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity
{
    public class AppUser : IdentityUser<int>
    {
        public List<ChatEntity> Chats { get; set; } = new List<ChatEntity>();
    }
}
