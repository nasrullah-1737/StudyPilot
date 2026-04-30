using StudyPilot.Models;

namespace StudyPilot.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResult> LoginAsync(string email, string password);
    Task<AuthResult> RegisterAsync(string name, string email, string password);
    Task LogoutAsync();
    Task<bool> IsAuthenticatedAsync();
    Task<UserAccount?> GetCurrentUserAsync();
}
