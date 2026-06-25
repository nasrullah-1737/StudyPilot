using StudyPilot.ViewModels;

namespace StudyPilot.Views;

public partial class MorePage : ContentPage
{
    private readonly MoreViewModel _viewModel;

    public MorePage(MoreViewModel viewModel)
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
