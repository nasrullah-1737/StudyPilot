using StudyPilot.Models;

namespace StudyPilot.Services.Interfaces;

public interface IFileService
{
    Task<IReadOnlyList<StudyFileItem>> GetAllAsync();
    Task<StudyFileItem?> PickAndStoreAsync();
    Task OpenAsync(StudyFileItem fileItem);
    Task DeleteAsync(StudyFileItem fileItem);
}
