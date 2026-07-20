using Application.Interfaces;

namespace Infrastructure.Services
{
    public class PasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            // Băm mật khẩu với BCrypt
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string passwordHash)
        {
            // So sánh mật khẩu người dùng nhập vào với mã hash trong Database
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
    }
}
