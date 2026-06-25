using StudyPilot.Services.Interfaces;

namespace StudyPilot.Views;

public partial class ThemedAlertPage : ContentPage
{
    private readonly TaskCompletionSource<bool> _tcs = new();
    private readonly bool _isConfirm;

    public ThemedAlertPage(
        string title,
        string message,
        bool isConfirm,
        string accept,
        string? cancel,
        AlertTone tone)
    {
        if (Application.Current?.Resources is ResourceDictionary appResources)
        {
            Resources.MergedDictionaries.Add(appResources);
        }

        InitializeComponent();

        _isConfirm = isConfirm;
        TitleLabel.Text = title;
        MessageLabel.Text = message;
        AcceptButton.Text = accept;

        ApplyTone(tone);

        if (isConfirm && !string.IsNullOrWhiteSpace(cancel))
        {
            CancelButton.Text = cancel;
            CancelButton.IsVisible = true;
            Grid.SetColumn(AcceptButton, 1);
            Grid.SetColumnSpan(AcceptButton, 1);
        }
        else
        {
            CancelButton.IsVisible = false;
            Grid.SetColumn(AcceptButton, 0);
            Grid.SetColumnSpan(AcceptButton, 2);
        }
    }

    public Task<bool> WaitForResultAsync() => _tcs.Task;

    private void ApplyTone(AlertTone tone)
    {
        AccentBar.BackgroundColor = tone switch
        {
            AlertTone.Success => Color.FromArgb("#10B981"),
            AlertTone.Warning => Color.FromArgb("#F59E0B"),
            AlertTone.Error => Color.FromArgb("#EF4444"),
            _ => Color.FromArgb("#6366F1")
        };

        if (!_isConfirm)
        {
            return;
        }

        if (tone == AlertTone.Error)
        {
            AcceptButton.BackgroundColor = Color.FromArgb("#EF4444");
            AcceptButton.TextColor = Colors.White;
            AcceptButton.BorderWidth = 0;
        }
    }

    private async void OnAcceptClicked(object? sender, EventArgs e)
    {
        await CloseAsync(true);
    }

    private async void OnCancelClicked(object? sender, EventArgs e)
    {
        await CloseAsync(false);
    }

    private async Task CloseAsync(bool result)
    {
        AcceptButton.IsEnabled = false;
        CancelButton.IsEnabled = false;

        if (Navigation.ModalStack.Count > 0)
        {
            await Navigation.PopModalAsync(false);
        }

        _tcs.TrySetResult(result);
    }
}
