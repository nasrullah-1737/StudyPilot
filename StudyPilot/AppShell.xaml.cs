using StudyPilot.Helpers;
using StudyPilot.Services.Interfaces;
using StudyPilot.Views;

namespace StudyPilot;

public partial class AppShell : Shell
{
    private readonly IAuthService _authService;
    private bool _isInitialized;

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

        if (_isInitialized)
        {
            return;
        }

        _isInitialized = true;
        var isAuthenticated = await _authService.IsAuthenticatedAsync();
        await GoToAsync(isAuthenticated ? AppRoutes.DashboardRoot : AppRoutes.LoginRoot, false);
    }
}
