using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ToDo.Domain.Entities;

namespace ToDo.Infrastructure.Persistence.Configurations
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {

        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(u => u.FirstName)
                .IsRequired()
                .HasMaxLength(50);
            builder.Property(u => u.LastName)
                .IsRequired()
                .HasMaxLength(50);
            builder.Property(u => u.CreatedAt)
                .IsRequired();
            builder.Property(u => u.IsActive)
                .IsRequired();
        }
    }
}
