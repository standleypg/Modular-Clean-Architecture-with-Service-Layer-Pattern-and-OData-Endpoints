using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RetailPortal.Model.Db.Entities;

namespace RetailPortal.Data.Db.Configurations;

public class RoleConfigurations : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");

        builder.HasKey(e => e.Id);

        builder.Property(a => a.Id)
            .ValueGeneratedOnAdd();

        builder.Property(a => a.Guid)
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(50);
    }
}