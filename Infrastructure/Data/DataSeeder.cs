using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Data
{
    public static class DataSeeder
    {
        public static async Task SeedAdminAsync(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<AppDbContext>();
            var passwordHasher = serviceProvider.GetRequiredService<IPasswordHasher>();

            // Áp dụng các thay đổi Migration mới nhất vào DB (nếu có)
            await context.Database.MigrateAsync();

            // Kiểm tra xem đã có Admin nào trong hệ thống chưa
            if (!await context.Users.AnyAsync(u => u.Role == Role.Admin))
            {
                var defaultAdmin = new User
                {
                    FullName = "Bí thư Chi đoàn",
                    Gender = Gender.Male,
                    DateOfBirth = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    PhoneNumber = "0123456789", // Tài khoản mặc định
                    PasswordHash = passwordHasher.HashPassword("Admin@123"), // Mật khẩu mặc định
                    Position = Position.Secretary,
                    Role = Role.Admin
                };

                context.Users.Add(defaultAdmin);
                await context.SaveChangesAsync();
            }
        }
    }
}
