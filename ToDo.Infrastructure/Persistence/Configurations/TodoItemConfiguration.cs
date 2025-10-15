using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ToDo.Domain.Entities;

namespace ToDo.Infrastructure.Persistence.Configurations
{
    public class TodoItemConfiguration : IEntityTypeConfiguration<TodoItem>
    {
        public void Configure(EntityTypeBuilder<TodoItem> builder)
        {
            builder.Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(500);
            builder.Property(t => t.Description)
                .HasMaxLength(2000);
            builder.Property(t => t.IsCompleted)
                .IsRequired();
            builder.HasOne(t => t.Category)
                .WithMany(c => c.TodoItems)
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);
            builder.Property(t => t.CreatedAt)
                .IsRequired();
            builder.HasOne(t => t.User)
                .WithMany(u => u.TodoItems)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
