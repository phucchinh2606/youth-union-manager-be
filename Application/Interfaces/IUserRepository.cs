using Domain.Entities;

namespace Application.Interfaces
{
    public interface IUserRepository
    {
        // Tìm người dùng theo số điện thoại (dùng cho đăng nhập)
        Task<User?> GetByPhoneNumberAsync(string phoneNumber);

        // Kiểm tra xem số điện thoại đã tồn tại chưa (dùng cho đăng ký)
        Task<bool> ExistsByPhoneNumberAsync(string phoneNumber);

        // Thêm người dùng mới
        Task AddAsync(User user);

        // Lưu các thay đổi vào database
        Task SaveChangesAsync();

        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByIdAsync(Guid id);
        void Update(User user);
        void Delete(User user);
    }
}
