using StudyPilot.ViewModels;

namespace StudyPilot.Views;

public partial class NoteEditorPage : ContentPage
{
    public NoteEditorPage(NoteEditorViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
