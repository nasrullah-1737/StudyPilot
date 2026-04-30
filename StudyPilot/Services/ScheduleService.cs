using StudyPilot.Helpers;
using StudyPilot.Models;
using StudyPilot.Services.Interfaces;

namespace StudyPilot.Services;

public class ScheduleService : IScheduleService
{
    private readonly IDatabaseService _databaseService;
    private readonly INotificationService _notificationService;

    public ScheduleService(IDatabaseService databaseService, INotificationService notificationService)
    {
        _databaseService = databaseService;
        _notificationService = notificationService;
    }

    public async Task<IReadOnlyList<ClassScheduleItem>> GetAllAsync()
    {
        await _databaseService.InitializeAsync();
        return await _databaseService.Connection.Table<ClassScheduleItem>()
            .OrderBy(x => x.DayOfWeekNumber)
            .ThenBy(x => x.StartTime)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<ClassScheduleItem>> GetForTodayAsync()
    {
        await _databaseService.InitializeAsync();
        var dayNumber = DateHelper.ToAppDayOfWeek(DateTime.Now);
        return await _databaseService.Connection.Table<ClassScheduleItem>()
            .Where(x => x.DayOfWeekNumber == dayNumber)
            .OrderBy(x => x.StartTime)
            .ToListAsync();
    }

    public async Task<ClassScheduleItem?> GetByIdAsync(int id)
    {
        await _databaseService.InitializeAsync();
        return await _databaseService.Connection.FindAsync<ClassScheduleItem>(id);
    }

    public async Task<int> SaveAsync(ClassScheduleItem item)
    {
        await _databaseService.InitializeAsync();
        int changes;
        if (item.Id == 0)
        {
            changes = await _databaseService.Connection.InsertAsync(item);
        }
        else
        {
            changes = await _databaseService.Connection.UpdateAsync(item);
        }

        await _notificationService.ScheduleClassReminderAsync(item);
        return changes;
    }

    public async Task DeleteAsync(ClassScheduleItem item)
    {
        await _databaseService.InitializeAsync();
        await _databaseService.Connection.DeleteAsync(item);
    }
}
