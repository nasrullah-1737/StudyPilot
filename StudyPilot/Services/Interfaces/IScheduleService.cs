using StudyPilot.Models;

namespace StudyPilot.Services.Interfaces;

public interface IScheduleService
{
    Task<IReadOnlyList<ClassScheduleItem>> GetAllAsync();
    Task<IReadOnlyList<ClassScheduleItem>> GetForTodayAsync();
    Task<ClassScheduleItem?> GetByIdAsync(int id);
    Task<int> SaveAsync(ClassScheduleItem item);
    Task DeleteAsync(ClassScheduleItem item);
}
