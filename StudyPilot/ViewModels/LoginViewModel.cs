using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StudyPilot.Helpers;
using StudyPilot.Services.Interfaces;

namespace StudyPilot.ViewModels;

public partial class LoginViewModel : BaseViewModel
{
    private readonly IAuthService _authService;
    private readonly INotificationService _notificationService;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    public LoginViewModel(IAuthService authService, INotificationService notificationService)
    {
        _authService = authService;
        _notificationService = notificationService;
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
            await Shell.Current.DisplayAlert("Validation", "Email and password are required.", "OK");
            return;
        }

        IsBusy = true;
        try
        {
            var result = await _authService.LoginAsync(Email, Password);
            if (!result.Success)
            {
                await Shell.Current.DisplayAlert("Login Failed", result.Message, "OK");
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
