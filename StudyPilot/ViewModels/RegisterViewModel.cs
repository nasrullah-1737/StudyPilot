using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StudyPilot.Helpers;
using StudyPilot.Services.Interfaces;

namespace StudyPilot.ViewModels;

public partial class RegisterViewModel : BaseViewModel
{
    private readonly IAuthService _authService;
    private readonly IAlertService _alertService;

    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    [ObservableProperty]
    private string confirmPassword = string.Empty;

    public RegisterViewModel(IAuthService authService, IAlertService alertService)
    {
        _authService = authService;
        _alertService = alertService;
        Title = "Register";
    }

    [RelayCommand]
    private async Task RegisterAsync()
    {
        if (IsBusy)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            await _alertService.ShowAsync("Validation", "All fields are required.", tone: AlertTone.Warning);
            return;
        }

        if (Password != ConfirmPassword)
        {
            await _alertService.ShowAsync("Validation", "Passwords do not match.", tone: AlertTone.Warning);
            return;
        }

        IsBusy = true;
        try
        {
            var result = await _authService.RegisterAsync(Name, Email, Password);
            if (!result.Success)
            {
                await _alertService.ShowAsync("Registration Failed", result.Message, tone: AlertTone.Error);
                return;
            }

            await _alertService.ShowAsync("Success", "Account created successfully.", tone: AlertTone.Success);
            await Shell.Current.GoToAsync(AppRoutes.LoginRoot);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private Task BackToLoginAsync() => Shell.Current.GoToAsync(AppRoutes.LoginRoot);
}
