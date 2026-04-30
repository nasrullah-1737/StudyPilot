using StudyPilot.ViewModels;

namespace StudyPilot.Views;

public partial class FocusPage : ContentPage
{
    public FocusPage(FocusViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
