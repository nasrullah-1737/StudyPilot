using StudyPilot.Models;

namespace StudyPilot.Services.Interfaces;

public interface INoteService
{
    Task<IReadOnlyList<NoteItem>> GetAllAsync();
    Task<NoteItem?> GetByIdAsync(int id);
    Task<int> SaveAsync(NoteItem note);
    Task DeleteAsync(NoteItem note);
    Task<IReadOnlyList<string>> GetSubjectsAsync();
}
