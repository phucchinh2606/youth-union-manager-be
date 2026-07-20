using Application.DTOs.Auth;

namespace Application.Interfaces
{
    public interface IAuthService
    {
        // Hàm đăng ký: Trả về true nếu thành công
        Task<bool> RegisterAsync(RegisterRequestDto request);
        //hàm Login trả về AuthResponseDto
        Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
    }
}
