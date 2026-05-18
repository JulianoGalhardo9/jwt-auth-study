using IdentityMicroservice.Domain.Entities;
using Microsoft.EntityFrameworkCore; // ← Esse cara precisa estar aqui!
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdentityMicroservice.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(150);

        builder.HasIndex(u => u.Email).IsUnique();

        builder.Property(u => u.PasswordHash)
            .IsRequired();

        builder.Property(u => u.RefreshToken)
            .HasMaxLength(200);
    }
}