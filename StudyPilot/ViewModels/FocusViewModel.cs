using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace StudyPilot.ViewModels;

public partial class FocusViewModel : BaseViewModel
{
    private const int FocusSeconds = 25 * 60;
    private const int BreakSeconds = 5 * 60;

    private readonly IDispatcherTimer _timer;
    private int _remainingSeconds = FocusSeconds;
    private int _totalSeconds = FocusSeconds;
    private bool _isBreak;

    [ObservableProperty]
    private string timerText = "25:00";

    [ObservableProperty]
    private string sessionLabel = "Focus Session";

    [ObservableProperty]
    private string startPauseText = "Start";

    [ObservableProperty]
    private double progress = 1;

    public FocusViewModel()
    {
        Title = "Focus Mode";
        _timer = Application.Current!.Dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromSeconds(1);
        _timer.Tick += TimerTick;
        UpdateUi();
    }

    [RelayCommand]
    private void StartPause()
    {
        if (_timer.IsRunning)
        {
            _timer.Stop();
            StartPauseText = "Resume";
            return;
        }

        _timer.Start();
        StartPauseText = "Pause";
    }

    [RelayCommand]
    private void Reset()
    {
        _timer.Stop();
        _isBreak = false;
        _totalSeconds = FocusSeconds;
        _remainingSeconds = FocusSeconds;
        StartPauseText = "Start";
        UpdateUi();
    }

    private async void TimerTick(object? sender, EventArgs e)
    {
        if (_remainingSeconds > 0)
        {
            _remainingSeconds--;
            UpdateUi();
            return;
        }

        _isBreak = !_isBreak;
        _totalSeconds = _isBreak ? BreakSeconds : FocusSeconds;
        _remainingSeconds = _totalSeconds;
        UpdateUi();

        var message = _isBreak
            ? "Focus round complete. Time for a short break."
            : "Break complete. Back to focus.";
        await Shell.Current.DisplayAlert("Pomodoro", message, "OK");
    }

    private void UpdateUi()
    {
        TimerText = TimeSpan.FromSeconds(_remainingSeconds).ToString(@"mm\:ss");
        SessionLabel = _isBreak ? "Break Session" : "Focus Session";
        Progress = _totalSeconds == 0 ? 0 : (double)_remainingSeconds / _totalSeconds;
    }
}
