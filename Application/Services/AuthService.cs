using Application.DTOs.Auth;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator; // Thêm dòng này

        // Cập nhật Constructor
        public AuthService(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IJwtTokenGenerator jwtTokenGenerator)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
        {
            // 1. Tìm người dùng theo số điện thoại
            var user = await _userRepository.GetByPhoneNumberAsync(request.PhoneNumber);
            if (user == null)
            {
                throw new Exception("Số điện thoại hoặc mật khẩu không chính xác.");
            }

            // 2. Kiểm tra mật khẩu
            var isPasswordValid = _passwordHasher.VerifyPassword(request.Password, user.PasswordHash);
            if (!isPasswordValid)
            {
                throw new Exception("Số điện thoại hoặc mật khẩu không chính xác.");
            }

            // 3. Tạo Token
            var token = _jwtTokenGenerator.GenerateToken(user);

            // 4. Trả về kết quả
            return new AuthResponseDto
            {
                Token = token,
                FullName = user.FullName,
                Role = user.Role.ToString()
            };
        }

        public async Task<bool> RegisterAsync(RegisterRequestDto request)
        {
            // 1. Chặn đăng ký chức vụ Bí thư từ bên ngoài
            if (request.Position == Position.Secretary)
            {
                throw new Exception("Không thể đăng ký tài khoản với chức vụ Bí thư.");
            }

            // 2. Kiểm tra số điện thoại đã tồn tại chưa
            var isExists = await _userRepository.ExistsByPhoneNumberAsync(request.PhoneNumber);
            if (isExists)
            {
                throw new Exception("Số điện thoại này đã được đăng ký.");
            }

            // 3. Khởi tạo đối tượng User mới (Luôn gán Role là User)
            var user = new User
            {
                FullName = request.FullName,
                Gender = request.Gender,
                DateOfBirth = request.DateOfBirth,
                PhoneNumber = request.PhoneNumber,
                PasswordHash = _passwordHasher.HashPassword(request.Password),
                Position = request.Position,
                Role = Role.User // Ép cứng quyền là User
            };

            // 4. Lưu vào Database
            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            return true;
        }
    }
}
