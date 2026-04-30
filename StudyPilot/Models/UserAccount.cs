using SQLite;

namespace StudyPilot.Models;

[Table("Users")]
public class UserAccount
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [MaxLength(120), Unique]
    public string Email { get; set; } = string.Empty;

    [MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(256)]
    public string PasswordHash { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
