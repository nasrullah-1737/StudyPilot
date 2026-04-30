using StudyPilot.ViewModels;

namespace StudyPilot.Views;

public partial class ScheduleEditorPage : ContentPage
{
    public ScheduleEditorPage(ScheduleEditorViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
