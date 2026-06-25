using StudyPilot.Services.Interfaces;
using StudyPilot.Views;

namespace StudyPilot.Services;

public class ThemedAlertService : IAlertService
{
    public async Task ShowAsync(string title, string message, string accept = "OK", AlertTone tone = AlertTone.Info)
    {
        await PresentAsync(title, message, isConfirm: false, accept, cancel: null, tone);
    }

    public Task<bool> ConfirmAsync(
        string title,
        string message,
        string accept = "OK",
        string cancel = "Cancel",
        AlertTone tone = AlertTone.Confirm)
        => PresentAsync(title, message, isConfirm: true, accept, cancel, tone);

    private static Task<bool> PresentAsync(
        string title,
        string message,
        bool isConfirm,
        string accept,
        string? cancel,
        AlertTone tone)
    {
        if (Application.Current?.MainPage is null)
        {
            return Task.FromResult(false);
        }

        return MainThread.InvokeOnMainThreadAsync(async () =>
        {
            var page = new ThemedAlertPage(title, message, isConfirm, accept, cancel, tone);
            var navigation = Shell.Current?.Navigation ?? Application.Current!.MainPage!.Navigation;
            await navigation.PushModalAsync(page, false);
            return await page.WaitForResultAsync();
        });
    }
}
