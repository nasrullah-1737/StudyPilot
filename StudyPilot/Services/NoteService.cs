using StudyPilot.Models;
using StudyPilot.Services.Interfaces;

namespace StudyPilot.Services;

public class NoteService : INoteService
{
    private readonly IDatabaseService _databaseService;

    public NoteService(IDatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    public async Task<IReadOnlyList<NoteItem>> GetAllAsync()
    {
        await _databaseService.InitializeAsync();
        return await _databaseService.Connection.Table<NoteItem>()
            .OrderByDescending(x => x.UpdatedAt)
            .ToListAsync();
    }

    public async Task<NoteItem?> GetByIdAsync(int id)
    {
        await _databaseService.InitializeAsync();
        return await _databaseService.Connection.FindAsync<NoteItem>(id);
    }

    public async Task<int> SaveAsync(NoteItem note)
    {
        await _databaseService.InitializeAsync();
        note.UpdatedAt = DateTime.UtcNow;
        if (note.Id == 0)
        {
            note.CreatedAt = DateTime.UtcNow;
            return await _databaseService.Connection.InsertAsync(note);
        }

        return await _databaseService.Connection.UpdateAsync(note);
    }

    public async Task DeleteAsync(NoteItem note)
    {
        await _databaseService.InitializeAsync();
        await _databaseService.Connection.DeleteAsync(note);
    }

    public async Task<IReadOnlyList<string>> GetSubjectsAsync()
    {
        await _databaseService.InitializeAsync();
        var items = await _databaseService.Connection.Table<NoteItem>().ToListAsync();
        return items.Select(x => x.Subject)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(x => x)
            .ToList();
    }
}
