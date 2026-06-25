using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StudyPilot.Helpers;
using StudyPilot.Services.Interfaces;

namespace StudyPilot.ViewModels;

public partial class MoreViewModel : BaseViewModel
{
    private readonly IAuthService _authService;
    private readonly IAlertService _alertService;

    [ObservableProperty]
    private string userName = "Student";

    [ObservableProperty]
    private string userEmail = string.Empty;

    [ObservableProperty]
    private string userInitial = "S";

    [ObservableProperty]
    private string appVersion = "1.0";

    public MoreViewModel(IAuthService authService, IAlertService alertService)
    {
        _authService = authService;
        _alertService = alertService;
        Title = "More";
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        var user = await _authService.GetCurrentUserAsync();
        UserName = user?.Name ?? "Student";
        UserEmail = user?.Email ?? string.Empty;
        UserInitial = string.IsNullOrWhiteSpace(user?.Name)
            ? "S"
            : char.ToUpperInvariant(user.Name.Trim()[0]).ToString();
        AppVersion = AppInfo.Current.VersionString;
    }

    [RelayCommand]
    private async Task LogoutAsync()
    {
        var confirmed = await _alertService.ConfirmAsync(
            "Sign Out",
            "Are you sure you want to sign out of StudyPilot?",
            "Sign Out",
            "Cancel",
            AlertTone.Confirm);

        if (!confirmed)
        {
            return;
        }

        await _authService.LogoutAsync();
        await Shell.Current.GoToAsync(AppRoutes.LoginRoot);
    }
}
