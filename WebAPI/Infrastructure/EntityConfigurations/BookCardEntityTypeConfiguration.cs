using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebAPI.Models;

namespace WebAPI.Infrastructure.EntityConfigurations
{
    public class BookCardEntityTypeConfiguration
        : IEntityTypeConfiguration<Models.BookCard>
    {
        public void Configure(EntityTypeBuilder<Models.BookCard> builder)
        {
            builder.ToTable(nameof(Models.BookCard));

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired();

            builder.Property(x=>x.Author)
                .IsRequired();

            builder.Property(x => x.BinaryPhoto)
                .IsRequired(false);
        }
    }
}
