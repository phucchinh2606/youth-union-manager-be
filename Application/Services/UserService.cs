using Application.DTOs.User;
using Application.Interfaces;
using Domain.Entities;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        public UserService(IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<UserResponseDto> CreateUserAsync(CreateUserRequestDto request)
        {
            // 1. Kiểm tra số điện thoại trùng lặp
            var isExists = await _userRepository.ExistsByPhoneNumberAsync(request.PhoneNumber);
            if (isExists)
            {
                throw new Exception("Số điện thoại này đã tồn tại trong hệ thống.");
            }

            // 2. Tạo Entity
            var user = new User
            {
                FullName = request.FullName,
                Gender = request.Gender,
                DateOfBirth = request.DateOfBirth,
                PhoneNumber = request.PhoneNumber,
                PasswordHash = _passwordHasher.HashPassword(request.Password), // Băm mật khẩu
                Position = request.Position,
                Role = request.Role
            };

            // 3. Lưu vào DB
            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            // 4. Trả về thông tin user vừa tạo (không trả về Password)
            return new UserResponseDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Gender = user.Gender.ToString(),
                DateOfBirth = user.DateOfBirth,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role.ToString(),
                Position = user.Position.ToString(),
                CreatedAt = user.CreatedAt
            };
        }

        public async Task<IEnumerable<UserResponseDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();

            // Chuyển đổi từ Entity sang DTO
            return users.Select(u => new UserResponseDto
            {
                Id = u.Id,
                FullName = u.FullName,
                Gender = u.Gender.ToString(),
                DateOfBirth = u.DateOfBirth,
                PhoneNumber = u.PhoneNumber,
                Role = u.Role.ToString(),
                Position = u.Position.ToString(),
                CreatedAt = u.CreatedAt
            });
        }

        public async Task<UserResponseDto?> GetUserByIdAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return null;

            return new UserResponseDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Gender = user.Gender.ToString(),
                DateOfBirth = user.DateOfBirth,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role.ToString(),
                Position = user.Position.ToString(),
                CreatedAt = user.CreatedAt
            };
        }

        public async Task<UserResponseDto> UpdateUserAsync(Guid id, UpdateUserRequestDto request)
        {
            // 1. Tìm user cần update
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                throw new Exception("Không tìm thấy đoàn viên cần cập nhật.");
            }

            // 2. Kiểm tra nếu số điện thoại bị thay đổi, số mới đã có ai dùng chưa?
            if (user.PhoneNumber != request.PhoneNumber)
            {
                var isPhoneNumberExists = await _userRepository.ExistsByPhoneNumberAsync(request.PhoneNumber);
                if (isPhoneNumberExists)
                {
                    throw new Exception("Số điện thoại này đã được sử dụng bởi một tài khoản khác.");
                }
            }

            // 3. Cập nhật các trường thông tin
            user.FullName = request.FullName;
            user.Gender = request.Gender;
            user.DateOfBirth = request.DateOfBirth;
            user.PhoneNumber = request.PhoneNumber;
            user.Position = request.Position;
            user.Role = request.Role;
            user.UpdatedAt = DateTime.UtcNow; // Ghi nhận thời gian sửa

            // 4. Lưu thay đổi
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();

            // 5. Trả về thông tin sau khi update
            return new UserResponseDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Gender = user.Gender.ToString(),
                DateOfBirth = user.DateOfBirth,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role.ToString(),
                Position = user.Position.ToString(),
                CreatedAt = user.CreatedAt
            };
        }

        public async Task<bool> DeleteUserAsync(Guid id)
        {
            // 1. Tìm user cần xóa
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                throw new Exception("Không tìm thấy đoàn viên cần xóa.");
            }

            // 2. Chặn việc xóa tài khoản Admin gốc (có thể chặn thêm việc Admin tự xóa chính mình nếu cần)
            if (user.PhoneNumber == "0123456789")
            {
                throw new Exception("Không thể xóa tài khoản Quản trị viên gốc của hệ thống.");
            }

            // 3. Thực hiện xóa
            _userRepository.Delete(user);
            await _userRepository.SaveChangesAsync();

            return true;
        }
    }
}
