using Mais_Kitchen.Models;

namespace Mais_Kitchen.Services
{
    public interface IAccountService
    {
        Task<User?> AuthenticateAsync(string email, string password);
        Task<(bool Success, string? Error)> RegisterAsync(User user, string password);
        Task<User?> GetByIdAsync(int userId);
        Task<User?> GetByEmailAsync(string email);
        Task GeneratePasswordResetTokenAsync(User user);
        Task<bool> ResetPasswordAsync(string email, string token, string newPassword);
    }
}
