using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StudyPilot.Helpers;
using StudyPilot.Models;
using StudyPilot.Services.Interfaces;
using System.Collections.ObjectModel;

namespace StudyPilot.ViewModels;

public partial class TimetableViewModel : BaseViewModel
{
    private readonly IScheduleService _scheduleService;
    private readonly IReadOnlyDictionary<int, string> _dayMap = new Dictionary<int, string>
    {
        [1] = "Monday",
        [2] = "Tuesday",
        [3] = "Wednesday",
        [4] = "Thursday",
        [5] = "Friday",
        [6] = "Saturday",
        [7] = "Sunday"
    };

    public ObservableCollection<ClassScheduleItem> TodayClasses { get; } = [];
    public ObservableCollection<ScheduleDisplayItem> WeeklyClasses { get; } = [];

    [ObservableProperty]
    private string todayTitle = "Today's Classes";

    public TimetableViewModel(IScheduleService scheduleService)
    {
        _scheduleService = scheduleService;
        Title = "Timetable";
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        if (IsBusy)
        {
            return;
        }

        IsBusy = true;
        try
        {
            var today = DateHelper.ToAppDayOfWeek(DateTime.Now);
            TodayTitle = $"{_dayMap[today]} Classes";

            TodayClasses.Clear();
            foreach (var item in await _scheduleService.GetForTodayAsync())
            {
                TodayClasses.Add(item);
            }

            var all = await _scheduleService.GetAllAsync();
            WeeklyClasses.Clear();
            foreach (var item in all.OrderBy(x => x.DayOfWeekNumber).ThenBy(x => x.StartTime))
            {
                WeeklyClasses.Add(new ScheduleDisplayItem
                {
                    Id = item.Id,
                    DayName = _dayMap[item.DayOfWeekNumber],
                    Subject = item.Subject,
                    Room = item.Room,
                    StartTime = item.StartTime,
                    EndTime = item.EndTime
                });
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private Task AddAsync() => Shell.Current.GoToAsync(AppRoutes.ScheduleEditor);

    [RelayCommand]
    private Task EditAsync(ScheduleDisplayItem item) => Shell.Current.GoToAsync($"{AppRoutes.ScheduleEditor}?scheduleId={item.Id}");

    [RelayCommand]
    private async Task DeleteAsync(ScheduleDisplayItem item)
    {
        if (item is null)
        {
            return;
        }

        var confirmed = await Shell.Current.DisplayAlert("Delete Class", "Delete this class slot?", "Delete", "Cancel");
        if (!confirmed)
        {
            return;
        }

        var model = await _scheduleService.GetByIdAsync(item.Id);
        if (model is null)
        {
            return;
        }

        await _scheduleService.DeleteAsync(model);
        await LoadAsync();
    }

    public class ScheduleDisplayItem
    {
        public int Id { get; set; }
        public string DayName { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Room { get; set; } = string.Empty;
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}
