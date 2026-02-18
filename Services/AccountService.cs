using Mais_Kitchen.Data;
using Mais_Kitchen.Models;
using Microsoft.EntityFrameworkCore;

namespace Mais_Kitchen.Services
{
    public class AccountService : IAccountService
    {
        private readonly ApplicationDbContext _context;

        public AccountService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User?> AuthenticateAsync(string email, string password)
        {
            var normalizedEmail = email.Trim().ToLower();

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail && u.IsActive);

            if (user == null)
                return null;

            var valid = BCrypt.Net.BCrypt.Verify(password, user.Password);

            if (!valid)
                return null;

            return user;
        }

        public async Task<(bool Success, string? Error)> RegisterAsync(User user, string password)
        {
            var normalizedEmail = user.Email.Trim().ToLower();

            var exists = await _context.Users
                .AnyAsync(u => u.Email.ToLower() == normalizedEmail);

            if (exists)
                return (false, "Email already exists.");

            user.Email = normalizedEmail;
            user.Password = BCrypt.Net.BCrypt.HashPassword(password);
            user.CreatedDate = DateTime.UtcNow;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return (true, null);
        }

        public async Task<User?> GetByIdAsync(int userId)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.UserID == userId);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            var normalizedEmail = email.Trim().ToLower();

            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail);
        }

        public async Task GeneratePasswordResetTokenAsync(User user)
        {
            user.PasswordResetToken =
                Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(64));

            user.PasswordResetExpiry = DateTime.UtcNow.AddHours(1);

            await _context.SaveChangesAsync();
        }

        public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword)
        {
            var normalizedEmail = email.Trim().ToLower();

            var user = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.Email.ToLower() == normalizedEmail &&
                    u.PasswordResetToken == token &&
                    u.PasswordResetExpiry > DateTime.UtcNow);

            if (user == null)
                return false;

            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.PasswordResetToken = null;
            user.PasswordResetExpiry = null;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
