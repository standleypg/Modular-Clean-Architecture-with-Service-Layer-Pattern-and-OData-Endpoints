using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RetailPortal.Model.Db.Entities;

namespace RetailPortal.Data.Db.Configurations;

public class ProductConfigurations: IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(e => e.Id);

        builder.Property(u => u.Id)
            .ValueGeneratedOnAdd()
            .UseIdentityColumn();

        builder.Property(a => a.Guid)
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.Description)
            .IsRequired()
            .HasMaxLength(1000);

        builder.OwnsOne(p=>p.Price, productBuilder =>
        {
            productBuilder.Property(p => p.Value)
                .HasColumnName("Amount")
                .IsRequired();

            productBuilder.Property(p => p.Currency)
                .HasColumnName("Currency")
                .IsRequired()
                .HasMaxLength(3);

            productBuilder.ToTable("Products");
        });

        builder.Property(p => p.ImageUrl)
            .HasDefaultValue(null)
            .IsRequired(false)
            .HasMaxLength(200);

        builder.Property(p => p.UserId)
            .IsRequired();
    }
}