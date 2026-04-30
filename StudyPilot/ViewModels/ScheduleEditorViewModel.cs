using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StudyPilot.Models;
using StudyPilot.Services.Interfaces;

namespace StudyPilot.ViewModels;

public partial class ScheduleEditorViewModel : BaseViewModel, IQueryAttributable
{
    private readonly IScheduleService _scheduleService;
    private int _scheduleId;

    [ObservableProperty]
    private string pageTitle = "New Class";

    [ObservableProperty]
    private string subject = string.Empty;

    [ObservableProperty]
    private string room = string.Empty;

    [ObservableProperty]
    private DayOption selectedDay = DayOption.Days[0];

    [ObservableProperty]
    private TimeSpan startTime = new(9, 0, 0);

    [ObservableProperty]
    private TimeSpan endTime = new(10, 0, 0);

    public IReadOnlyList<DayOption> Days => DayOption.Days;

    public ScheduleEditorViewModel(IScheduleService scheduleService)
    {
        _scheduleService = scheduleService;
        Title = "Class Editor";
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        _scheduleId = 0;
        if (query.TryGetValue("scheduleId", out var rawId) && int.TryParse(rawId?.ToString(), out var id))
        {
            _scheduleId = id;
        }

        if (_scheduleId <= 0)
        {
            ResetForCreate();
            return;
        }

        var model = await _scheduleService.GetByIdAsync(_scheduleId);
        if (model is null)
        {
            ResetForCreate();
            return;
        }

        PageTitle = "Edit Class";
        Subject = model.Subject;
        Room = model.Room;
        SelectedDay = Days.First(x => x.Number == model.DayOfWeekNumber);
        StartTime = model.StartTime;
        EndTime = model.EndTime;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(Subject))
        {
            await Shell.Current.DisplayAlert("Validation", "Subject is required.", "OK");
            return;
        }

        if (EndTime <= StartTime)
        {
            await Shell.Current.DisplayAlert("Validation", "End time must be after start time.", "OK");
            return;
        }

        var item = _scheduleId > 0
            ? await _scheduleService.GetByIdAsync(_scheduleId) ?? new ClassScheduleItem()
            : new ClassScheduleItem();

        item.Subject = Subject.Trim();
        item.Room = Room.Trim();
        item.DayOfWeekNumber = SelectedDay.Number;
        item.StartTime = StartTime;
        item.EndTime = EndTime;

        await _scheduleService.SaveAsync(item);
        await Shell.Current.GoToAsync("..");
    }

    private void ResetForCreate()
    {
        PageTitle = "New Class";
        Subject = string.Empty;
        Room = string.Empty;
        SelectedDay = DayOption.Days[0];
        StartTime = new TimeSpan(9, 0, 0);
        EndTime = new TimeSpan(10, 0, 0);
    }

    public record DayOption(int Number, string Name)
    {
        public static IReadOnlyList<DayOption> Days { get; } =
        [
            new(1, "Monday"),
            new(2, "Tuesday"),
            new(3, "Wednesday"),
            new(4, "Thursday"),
            new(5, "Friday"),
            new(6, "Saturday"),
            new(7, "Sunday")
        ];
    }
}
