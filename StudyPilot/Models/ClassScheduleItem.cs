using SQLite;

namespace StudyPilot.Models;

[Table("ClassSchedules")]
public class ClassScheduleItem
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [MaxLength(120)]
    public string Subject { get; set; } = string.Empty;

    [MaxLength(80)]
    public string Room { get; set; } = string.Empty;

    public int DayOfWeekNumber { get; set; } = 1;

    public TimeSpan StartTime { get; set; } = new(9, 0, 0);

    public TimeSpan EndTime { get; set; } = new(10, 0, 0);
}
