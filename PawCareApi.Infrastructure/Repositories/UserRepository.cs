using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PawCareApi.Domain.Entities;
using PawCareApi.Domain.Interfaces;
using PawCareApi.Infrastructure.Data;

namespace PawCareApi.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly PawCareContext _context;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(PawCareContext context, ILogger<UserRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<User?> ObterPorUsernameAsync(string username)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Username.ToLower() == username.Trim().ToLower());
        }

        public async Task<User?> ValidarCredenciaisAsync(string username, string password)
        {
            var user = await ObterPorUsernameAsync(username);
            if (user == null)
            {
                _logger.LogWarning("Validação de usuário falhou: usuário '{Username}' não existe.", username);
                return null;
            }

            var passwordValid = DbInitializer.VerifyPassword(password, user.PasswordHash);
            if (!passwordValid)
            {
                _logger.LogWarning("Validação de usuário falhou: senha incorreta para '{Username}'.", username);
                return null;
            }

            return user;
        }

        public async Task<User> CriarAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }
    }
}

