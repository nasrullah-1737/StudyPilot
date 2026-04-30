using StudyPilot.Services.Interfaces;

namespace StudyPilot;

public partial class App : Application
{
    public App(AppShell appShell, ISampleDataService sampleDataService)
    {
        InitializeComponent();
        MainPage = appShell;
        UserAppTheme = AppTheme.Light;
        _ = InitializeAsync(sampleDataService);
    }

    private static async Task InitializeAsync(ISampleDataService sampleDataService)
    {
        try
        {
            await sampleDataService.SeedAsync();
        }
        catch
        {
            // The app should still boot even if seed fails.
        }
    }
}
