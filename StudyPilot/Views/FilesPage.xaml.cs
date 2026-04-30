using StudyPilot.ViewModels;

namespace StudyPilot.Views;

public partial class FilesPage : ContentPage
{
    private readonly FilesViewModel _viewModel;

    public FilesPage(FilesViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadCommand.ExecuteAsync(null);
    }
}
