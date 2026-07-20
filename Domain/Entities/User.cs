using Domain.Enums;

namespace Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string FullName { get; set; }
        public required DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; } = Gender.Male;
        public required string PhoneNumber { get; set; }
        public required string PasswordHash { get; set; }
        public Role Role { get; set; } = Role.User;
        public Position Position { get; set; } = Position.Member;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
