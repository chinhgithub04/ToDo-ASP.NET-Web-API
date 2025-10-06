namespace ToDo.Domain.Entities
{
    public class Category
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }

        public string UserId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public ApplicationUser User { get; set; }
        public ICollection<TodoItem> TodoItems { get; set; }

    }
}
