using Microsoft.AspNetCore.Identity;

namespace ToDo.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? AvatarUrl { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public DateTimeOffset? LastLoginAt { get; set; }
        public bool IsActive { get; set; }
        public ICollection<Category> Categories { get; set; }
        public ICollection<TodoItem> TodoItems { get; set; }
    }
}
