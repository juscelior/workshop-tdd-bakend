using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wiz.Biblioteca.Domain.Models;

namespace Wiz.Biblioteca.Infra.Mappings
{
    public class AddressMap : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.ToTable("Address", "dbo");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.CEP)
                .HasColumnType("VARCHAR(8)")
                .HasMaxLength(8)
                .IsRequired();

            builder.HasMany(x => x.Customers)
                .WithOne(x => x.Address)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
