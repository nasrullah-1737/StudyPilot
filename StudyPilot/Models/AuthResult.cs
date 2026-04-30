namespace StudyPilot.Models;

public class AuthResult
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
    public UserAccount? User { get; init; }

    public static AuthResult Ok(UserAccount user, string message = "Success")
        => new() { Success = true, Message = message, User = user };

    public static AuthResult Fail(string message)
        => new() { Success = false, Message = message };
}
