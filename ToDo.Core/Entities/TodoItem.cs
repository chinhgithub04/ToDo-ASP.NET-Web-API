namespace ToDo.Domain.Entities
{
    public class TodoItem
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public bool IsCompleted { get; set; }

        public Guid? CategoryId { get; set; }
        public Category Category { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
