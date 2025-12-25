using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RetailPortal.Model.Db.Entities;

namespace RetailPortal.Data.Db.Configurations;

public class UserConfigurations : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(e => e.Id);

        builder.Property(a => a.Id)
            .ValueGeneratedOnAdd();

        builder.Property(a => a.Guid)
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(50);

        builder.OwnsOne(u=>u.Password, passwordBuilder =>
        {
            passwordBuilder.Property(p => p.PasswordHash)
                .HasColumnName("PasswordHash")
                .IsRequired(false)
                .HasDefaultValue(null);

            passwordBuilder.Property(p => p.PasswordSalt)
                .HasColumnName("PasswordSalt")
                .IsRequired(false)
                .HasDefaultValue(null);

            passwordBuilder.ToTable("Users");
        });

        builder.Property(u => u.TokenProvider)
            .IsRequired()
            .HasConversion<string>();

        builder.HasMany(u=>u.Addresses)
            .WithOne(a=>a.User)
            .HasForeignKey(a=>a.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.Roles)
            .WithMany(r => r.Users)
            .UsingEntity(
                "UserRoles",
                l => l.HasOne(typeof(Role)).WithMany().HasForeignKey("RoleId").HasPrincipalKey(nameof(Role.Id)),
                r => r.HasOne(typeof(User)).WithMany().HasForeignKey("UserId").HasPrincipalKey(nameof(User.Id)),
                j =>
                {
                    j.HasKey("UserId", "RoleId");
                    j.Property<DateTime>("AssignedDate").HasDefaultValueSql("NOW()");
                });
    }
}