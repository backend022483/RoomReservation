using Microsoft.AspNetCore.Identity;

namespace RoomResertionApp.Models
{
    public static class PasswordHelper
    {
        private static readonly PasswordHasher<string> _passwordHasher = new PasswordHasher<string>();

        public static string HashPassword(string password)
        {
            return _passwordHasher.HashPassword(null, password);
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            var result = _passwordHasher.VerifyHashedPassword(null, hashedPassword, password);
            return result == PasswordVerificationResult.Success;
        }
    }
}
