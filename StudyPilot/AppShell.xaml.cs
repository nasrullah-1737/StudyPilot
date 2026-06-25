using StudyPilot.Helpers;
using StudyPilot.Services.Interfaces;
using StudyPilot.Views;

namespace StudyPilot;

public partial class AppShell : Shell
{
    private readonly IAuthService _authService;
    private bool _isInitialized;
    private bool _themeApplied;

    public AppShell(IAuthService authService)
    {
        InitializeComponent();
        _authService = authService;

        Routing.RegisterRoute(AppRoutes.NoteEditor, typeof(NoteEditorPage));
        Routing.RegisterRoute(AppRoutes.TaskEditor, typeof(TaskEditorPage));
        Routing.RegisterRoute(AppRoutes.ScheduleEditor, typeof(ScheduleEditorPage));
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (!_themeApplied)
        {
            _themeApplied = true;
            ApplyThemeColors();
        }

        if (_isInitialized)
        {
            return;
        }

        _isInitialized = true;

        try
        {
            await Dispatcher.DispatchAsync(async () =>
            {
                var isAuthenticated = await _authService.IsAuthenticatedAsync();
                await GoToAsync(isAuthenticated ? AppRoutes.DashboardRoot : AppRoutes.LoginRoot, false);
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Startup navigation failed: {ex}");
            await GoToAsync(AppRoutes.LoginRoot, false);
        }
    }

    private void ApplyThemeColors()
    {
        var resources = Application.Current?.Resources;
        if (resources is null)
        {
            return;
        }

        Shell.SetTabBarBackgroundColor(this, GetColor(resources, "Surface", "#FFFFFF"));
        Shell.SetTabBarTitleColor(this, GetColor(resources, "Primary", "#6366F1"));
        Shell.SetTabBarForegroundColor(this, GetColor(resources, "Primary", "#6366F1"));
        Shell.SetTabBarUnselectedColor(this, GetColor(resources, "TextMuted", "#94A3B8"));
    }

    private static Color GetColor(ResourceDictionary resources, string key, string fallbackHex)
    {
        if (resources.TryGetValue(key, out var value) && value is Color color)
        {
            return color;
        }

        return Color.FromArgb(fallbackHex);
    }
}
