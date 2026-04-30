using StudyPilot.Helpers;
using StudyPilot.Models;
using StudyPilot.Services.Interfaces;

namespace StudyPilot.Services;

public class AuthService : IAuthService
{
    private readonly IDatabaseService _databaseService;

    public AuthService(IDatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    public async Task<AuthResult> LoginAsync(string email, string password)
    {
        await _databaseService.InitializeAsync();
        var normalized = email.Trim().ToLowerInvariant();
        var hash = SecurityHelper.HashPassword(password);

        var user = await _databaseService.Connection.Table<UserAccount>()
            .Where(x => x.Email == normalized && x.PasswordHash == hash)
            .FirstOrDefaultAsync();

        if (user is null)
        {
            return AuthResult.Fail("Invalid email or password.");
        }

        Preferences.Default.Set(AppConstants.SessionUserIdKey, user.Id);
        return AuthResult.Ok(user, "Welcome back.");
    }

    public async Task<AuthResult> RegisterAsync(string name, string email, string password)
    {
        await _databaseService.InitializeAsync();
        var normalized = email.Trim().ToLowerInvariant();

        var exists = await _databaseService.Connection.Table<UserAccount>()
            .Where(x => x.Email == normalized)
            .FirstOrDefaultAsync();

        if (exists is not null)
        {
            return AuthResult.Fail("Email is already registered.");
        }

        var user = new UserAccount
        {
            Name = name.Trim(),
            Email = normalized,
            PasswordHash = SecurityHelper.HashPassword(password)
        };

        await _databaseService.Connection.InsertAsync(user);

        return AuthResult.Ok(user, "Account created.");
    }

    public Task LogoutAsync()
    {
        Preferences.Default.Remove(AppConstants.SessionUserIdKey);
        return Task.CompletedTask;
    }

    public async Task<bool> IsAuthenticatedAsync()
        => await GetCurrentUserAsync() is not null;

    public async Task<UserAccount?> GetCurrentUserAsync()
    {
        await _databaseService.InitializeAsync();
        var userId = Preferences.Default.Get(AppConstants.SessionUserIdKey, 0);
        if (userId <= 0)
        {
            return null;
        }

        return await _databaseService.Connection.FindAsync<UserAccount>(userId);
    }
}
