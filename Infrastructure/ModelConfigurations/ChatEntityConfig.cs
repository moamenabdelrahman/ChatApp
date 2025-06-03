using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.ModelConfigurations
{
    public class ChatEntityConfig : IEntityTypeConfiguration<ChatEntity>
    {
        public void Configure(EntityTypeBuilder<ChatEntity> builder)
        {
            builder.HasKey(c => c.Id);

            builder.HasMany<MessageEntity>(c => c.Messages)
                   .WithOne()
                   .HasForeignKey(msg => msg.ChatId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.NoAction);

            builder.Property(c => c.Type)
                   .HasConversion<int>();
        }
    }
}
