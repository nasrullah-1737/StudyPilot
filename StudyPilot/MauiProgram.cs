using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Plugin.LocalNotification;
using StudyPilot.Services;
using StudyPilot.Services.Interfaces;
using StudyPilot.ViewModels;
using StudyPilot.Views;

namespace StudyPilot;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseLocalNotification()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        RegisterServices(builder.Services);
        RegisterViewModels(builder.Services);
        RegisterViews(builder.Services);

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }

    private static void RegisterServices(IServiceCollection services)
    {
        services.AddSingleton<IDatabaseService, DatabaseService>();
        services.AddSingleton<StudyPilot.Services.Interfaces.INotificationService, NotificationService>();
        services.AddSingleton<IAuthService, AuthService>();
        services.AddSingleton<INoteService, NoteService>();
        services.AddSingleton<ITaskService, TaskService>();
        services.AddSingleton<IScheduleService, ScheduleService>();
        services.AddSingleton<IFileService, FileService>();
        services.AddSingleton<ISampleDataService, SampleDataService>();
    }

    private static void RegisterViewModels(IServiceCollection services)
    {
        services.AddTransient<LoginViewModel>();
        services.AddTransient<RegisterViewModel>();
        services.AddTransient<DashboardViewModel>();
        services.AddTransient<NotesViewModel>();
        services.AddTransient<NoteEditorViewModel>();
        services.AddTransient<TasksViewModel>();
        services.AddTransient<TaskEditorViewModel>();
        services.AddTransient<TimetableViewModel>();
        services.AddTransient<ScheduleEditorViewModel>();
        services.AddTransient<FocusViewModel>();
        services.AddTransient<FilesViewModel>();
    }

    private static void RegisterViews(IServiceCollection services)
    {
        services.AddSingleton<AppShell>();
        services.AddTransient<LoginPage>();
        services.AddTransient<RegisterPage>();
        services.AddTransient<DashboardPage>();
        services.AddTransient<NotesPage>();
        services.AddTransient<NoteEditorPage>();
        services.AddTransient<TasksPage>();
        services.AddTransient<TaskEditorPage>();
        services.AddTransient<TimetablePage>();
        services.AddTransient<ScheduleEditorPage>();
        services.AddTransient<FocusPage>();
        services.AddTransient<FilesPage>();
    }
}
