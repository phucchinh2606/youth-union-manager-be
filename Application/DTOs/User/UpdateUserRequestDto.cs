using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.User
{
    public class UpdateUserRequestDto
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

        [Required(ErrorMessage = "Vui lòng chọn chức vụ")]
        public Position Position { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn quyền hạn")]
        public Role Role { get; set; }
    }
}
