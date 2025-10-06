using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDo.Domain.Entities;
using ToDo.Infrastructure.Persistence.Configurations;

namespace ToDo.Infrastructure.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<TodoItem> TodoItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new ApplicationUserConfiguration());
            builder.ApplyConfiguration(new CategoryConfiguration());
            builder.ApplyConfiguration(new TodoItemConfiguration());
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries();

            foreach (var entry in entries)
            {
                if (entry.Entity is ApplicationUser user)
                {
                    if (entry.State == EntityState.Added)
                    {
                        user.CreatedAt = DateTimeOffset.UtcNow;
                        user.IsActive = true;
                    }
                    else if (entry.State == EntityState.Modified)
                    {
                        user.UpdatedAt = DateTimeOffset.UtcNow;
                    }
                }
                else if (entry.Entity is Category category)
                {
                    if (entry.State == EntityState.Added)
                    {
                        category.CreatedAt = DateTimeOffset.UtcNow;
                    }
                    else if (entry.State == EntityState.Modified)
                    {
                        category.UpdatedAt = DateTimeOffset.UtcNow;
                    }
                }
                else if (entry.Entity is TodoItem todo)
                {
                    if (entry.State == EntityState.Added)
                    {
                        todo.CreatedAt = DateTimeOffset.UtcNow;
                    }
                    else if (entry.State == EntityState.Modified)
                    {
                        todo.UpdatedAt = DateTimeOffset.UtcNow;
                    }
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }


    }
}
