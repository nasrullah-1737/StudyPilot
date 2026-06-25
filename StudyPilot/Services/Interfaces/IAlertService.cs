namespace StudyPilot.Services.Interfaces;

public interface IAlertService
{
    Task ShowAsync(string title, string message, string accept = "OK", AlertTone tone = AlertTone.Info);
    Task<bool> ConfirmAsync(string title, string message, string accept = "OK", string cancel = "Cancel", AlertTone tone = AlertTone.Confirm);
}

public enum AlertTone
{
    Info,
    Success,
    Warning,
    Error,
    Confirm
}
