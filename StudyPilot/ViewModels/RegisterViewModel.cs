using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StudyPilot.Helpers;
using StudyPilot.Services.Interfaces;

namespace StudyPilot.ViewModels;

public partial class RegisterViewModel : BaseViewModel
{
    private readonly IAuthService _authService;

    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    [ObservableProperty]
    private string confirmPassword = string.Empty;

    public RegisterViewModel(IAuthService authService)
    {
        _authService = authService;
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
            await Shell.Current.DisplayAlert("Validation", "All fields are required.", "OK");
            return;
        }

        if (Password != ConfirmPassword)
        {
            await Shell.Current.DisplayAlert("Validation", "Passwords do not match.", "OK");
            return;
        }

        IsBusy = true;
        try
        {
            var result = await _authService.RegisterAsync(Name, Email, Password);
            if (!result.Success)
            {
                await Shell.Current.DisplayAlert("Registration Failed", result.Message, "OK");
                return;
            }

            await Shell.Current.DisplayAlert("Success", "Account created successfully.", "OK");
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
