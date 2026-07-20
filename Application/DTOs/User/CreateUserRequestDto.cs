using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.User
{
    public class CreateUserRequestDto
    {
        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn giới tính")]
        public Gender Gender { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập ngày sinh")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn chức vụ")]
        public Position Position { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn quyền hạn")]
        public Role Role { get; set; } // Khác với Register, Admin tạo thì được chọn Role
    }
}
