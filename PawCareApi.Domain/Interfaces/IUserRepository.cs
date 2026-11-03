using PawCareApi.Domain.Entities;

namespace PawCareApi.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> ObterPorUsernameAsync(string username);
        Task<User?> ValidarCredenciaisAsync(string username, string password);
        Task<User> CriarAsync(User user);
    }
}

