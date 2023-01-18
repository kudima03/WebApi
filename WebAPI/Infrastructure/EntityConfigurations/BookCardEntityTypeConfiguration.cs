using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebAPI.Models;

namespace WebAPI.Infrastructure.EntityConfigurations
{
    public class BookCardEntityTypeConfiguration
        : IEntityTypeConfiguration<BookCard>
    {
        public void Configure(EntityTypeBuilder<BookCard> builder)
        {
            builder.ToTable(nameof(BookCard));

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
