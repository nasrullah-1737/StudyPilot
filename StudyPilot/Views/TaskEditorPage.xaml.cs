using StudyPilot.ViewModels;

namespace StudyPilot.Views;

public partial class TaskEditorPage : ContentPage
{
    public TaskEditorPage(TaskEditorViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
