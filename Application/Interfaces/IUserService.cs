using Application.DTOs.User;

namespace Application.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponseDto>> GetAllUsersAsync();
        Task<UserResponseDto?> GetUserByIdAsync(Guid id);
        Task<UserResponseDto> CreateUserAsync(CreateUserRequestDto request);
        Task<UserResponseDto> UpdateUserAsync(Guid id, UpdateUserRequestDto request);
        Task<bool> DeleteUserAsync(Guid id);
    }
}
