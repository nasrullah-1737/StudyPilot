using SQLite;
using StudyPilot.Helpers;
using StudyPilot.Models;
using StudyPilot.Services.Interfaces;

namespace StudyPilot.Services;

public class DatabaseService : IDatabaseService
{
    private readonly SemaphoreSlim _locker = new(1, 1);
    private bool _initialized;

    public SQLiteAsyncConnection Connection { get; }

    public DatabaseService()
    {
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, AppConstants.DatabaseFileName);
        Connection = new SQLiteAsyncConnection(dbPath);
    }

    public async Task InitializeAsync()
    {
        if (_initialized)
        {
            return;
        }

        await _locker.WaitAsync();
        try
        {
            if (_initialized)
            {
                return;
            }

            await Connection.CreateTableAsync<UserAccount>();
            await Connection.CreateTableAsync<NoteItem>();
            await Connection.CreateTableAsync<TaskItem>();
            await Connection.CreateTableAsync<ClassScheduleItem>();
            await Connection.CreateTableAsync<StudyFileItem>();

            _initialized = true;
        }
        finally
        {
            _locker.Release();
        }
    }
}
