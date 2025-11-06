namespace StudentActiveSystem.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public int GroupId { get; set; }
        public int? RoleId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        // Навигационные свойства
        public string? GroupName { get; set; }
        public string? RoleName { get; set; }
    }
}
