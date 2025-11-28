using HrMangmentSystem_Application.Interfaces.Auth;
using Microsoft.AspNetCore.Identity;

namespace HrMangmentSystem_Infrastructure.Implementations.Security
{
    public class PasswordHasherSer : IPasswordHasher
    {
        private readonly PasswordHasher<object> _passwordHasher = new();
        public string HashPassword(string password)
        {
            return _passwordHasher.HashPassword(user: null!, password);
        }

        public bool VerifyPassword(string hashedPassword, string providedPassword)
        {
            var result = _passwordHasher.VerifyHashedPassword(
                user: null!,
                hashedPassword: hashedPassword,
                providedPassword: providedPassword);

            return result == PasswordVerificationResult.Success
                   || result == PasswordVerificationResult.SuccessRehashNeeded;
        }
    }
}
