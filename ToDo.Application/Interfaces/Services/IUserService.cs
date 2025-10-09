namespace ToDo.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
    }
}
