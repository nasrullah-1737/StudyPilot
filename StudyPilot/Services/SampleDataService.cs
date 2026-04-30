using StudyPilot.Helpers;
using StudyPilot.Models;
using StudyPilot.Services.Interfaces;

namespace StudyPilot.Services;

public class SampleDataService : ISampleDataService
{
    private readonly IDatabaseService _databaseService;

    public SampleDataService(IDatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    public async Task SeedAsync()
    {
        await _databaseService.InitializeAsync();

        // Remove legacy demo account if it exists.
        var demoUser = await _databaseService.Connection.Table<UserAccount>()
            .Where(x => x.Email == "student@studypilot.app")
            .FirstOrDefaultAsync();
        if (demoUser is not null)
        {
            await _databaseService.Connection.DeleteAsync(demoUser);
        }

        if (Preferences.Default.Get(AppConstants.SeededKey, false))
        {
            return;
        }

        var existingNotes = await _databaseService.Connection.Table<NoteItem>().CountAsync();
        if (existingNotes == 0)
        {
            await _databaseService.Connection.InsertAllAsync(new List<NoteItem>
            {
                new() { Title = "Math Revision", Subject = "Mathematics", Content = "Revise calculus chapters 1-3." },
                new() { Title = "Chemistry Lab", Subject = "Chemistry", Content = "Bring lab coat and complete experiment sheet." },
                new() { Title = "English Essay", Subject = "English", Content = "Draft introduction and thesis statement." }
            });
        }

        var existingTasks = await _databaseService.Connection.Table<TaskItem>().CountAsync();
        if (existingTasks == 0)
        {
            await _databaseService.Connection.InsertAllAsync(new List<TaskItem>
            {
                new() { Title = "Physics Assignment", Description = "Solve chapter 4 questions", Deadline = DateTime.Now.AddHours(8), Priority = TaskPriority.High },
                new() { Title = "History Reading", Description = "Read pages 42-60", Deadline = DateTime.Now.AddDays(1), Priority = TaskPriority.Medium },
                new() { Title = "Prepare Slides", Description = "Team presentation draft", Deadline = DateTime.Now.AddDays(2), Priority = TaskPriority.Low }
            });
        }

        var existingSchedule = await _databaseService.Connection.Table<ClassScheduleItem>().CountAsync();
        if (existingSchedule == 0)
        {
            await _databaseService.Connection.InsertAllAsync(new List<ClassScheduleItem>
            {
                new() { Subject = "Mathematics", Room = "A-101", DayOfWeekNumber = 1, StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(10, 0, 0) },
                new() { Subject = "Computer Science", Room = "Lab-2", DayOfWeekNumber = 3, StartTime = new TimeSpan(11, 0, 0), EndTime = new TimeSpan(12, 30, 0) },
                new() { Subject = "Physics", Room = "B-204", DayOfWeekNumber = 5, StartTime = new TimeSpan(8, 30, 0), EndTime = new TimeSpan(9, 30, 0) }
            });
        }

        Preferences.Default.Set(AppConstants.SeededKey, true);
    }
}
