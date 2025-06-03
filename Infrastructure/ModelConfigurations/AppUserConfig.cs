using Infrastructure.Data;
using Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.ModelConfigurations
{
    public class AppUserConfig : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            builder.HasMany<ChatEntity>(u => u.Chats)
                   .WithMany(g => g.Members);

            builder.HasMany<MessageEntity>()
                   .WithOne(msg => msg.Sender)
                   .HasForeignKey(msg => msg.SenderId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
