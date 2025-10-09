namespace ToDo.Application.Interfaces.Services
{
    public interface ICategoryService
    {
        Task<bool> CategoryExistsAsync(Guid categoryId, CancellationToken cancellationToken = default);
    }
}
