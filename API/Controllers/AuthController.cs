using Application.DTOs.Auth;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            // Kiểm tra xem dữ liệu gửi lên có hợp lệ theo các rules (Required, MinLength,...) ở DTO không
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _authService.RegisterAsync(request);
                if (result)
                {
                    return Ok(new { message = "Đăng ký tài khoản thành công!" });
                }

                return BadRequest(new { message = "Đăng ký thất bại." });
            }
            catch (Exception ex)
            {
                // Bắt lỗi (ví dụ: Số điện thoại đã tồn tại, hoặc cố tình đăng ký Bí thư)
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _authService.LoginAsync(request);
                return Ok(result); // Trả về Token và thông tin user
            }
            catch (Exception ex)
            {
                // Trả về lỗi 401 Unauthorized nếu sai sđt/mật khẩu
                return Unauthorized(new { message = ex.Message });
            }
        }
    }
}
