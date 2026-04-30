using SQLite;

namespace StudyPilot.Services.Interfaces;

public interface IDatabaseService
{
    SQLiteAsyncConnection Connection { get; }
    Task InitializeAsync();
}
