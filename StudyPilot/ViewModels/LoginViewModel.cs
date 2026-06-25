using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StudyPilot.Helpers;
using StudyPilot.Services.Interfaces;

namespace StudyPilot.ViewModels;

public partial class LoginViewModel : BaseViewModel
{
    private readonly IAuthService _authService;
    private readonly INotificationService _notificationService;
    private readonly IAlertService _alertService;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    public LoginViewModel(IAuthService authService, INotificationService notificationService, IAlertService alertService)
    {
        _authService = authService;
        _notificationService = notificationService;
        _alertService = alertService;
        Title = "Login";
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (IsBusy)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            await _alertService.ShowAsync("Validation", "Email and password are required.", tone: AlertTone.Warning);
            return;
        }

        IsBusy = true;
        try
        {
            var result = await _authService.LoginAsync(Email, Password);
            if (!result.Success)
            {
                await _alertService.ShowAsync("Login Failed", result.Message, tone: AlertTone.Error);
                return;
            }

            await _notificationService.RequestPermissionAsync();
            await Shell.Current.GoToAsync(AppRoutes.DashboardRoot);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private Task OpenRegisterAsync() => Shell.Current.GoToAsync(AppRoutes.RegisterRoot);
}
