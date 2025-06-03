using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.ModelConfigurations
{
    public class MessageEntityConfig : IEntityTypeConfiguration<MessageEntity>
    {
        public void Configure(EntityTypeBuilder<MessageEntity> builder)
        {
            builder.HasKey(msg => msg.Id);

            builder.Property(msg => msg.TimeStamp)
                   .HasDefaultValueSql("GetDate()");
        }
    }
}
