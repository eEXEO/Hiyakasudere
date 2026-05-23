using ReactiveUI;
using System.Reactive;

namespace Hiyakasudere.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private ViewModelBase _currentView;
    private string _activeNav = "browse";

    public ViewModelBase CurrentView
    {
        get => _currentView;
        set => this.RaiseAndSetIfChanged(ref _currentView, value);
    }

    public string ActiveNav
    {
        get => _activeNav;
        set => this.RaiseAndSetIfChanged(ref _activeNav, value);
    }

    public ImageViewerViewModel ImageViewer { get; }

    public ReactiveCommand<Unit, Unit> NavigateBrowseCommand { get; }
    public ReactiveCommand<Unit, Unit> NavigateFavoritesCommand { get; }
    public ReactiveCommand<Unit, Unit> NavigateSettingsCommand { get; }

    private readonly BrowseViewModel _browseVm;
    private readonly FavoritesViewModel _favoritesVm;
    private readonly SettingsViewModel _settingsVm;

    public MainWindowViewModel(BrowseViewModel browseVm, FavoritesViewModel favoritesVm, 
        SettingsViewModel settingsVm, ImageViewerViewModel imageViewer)
    {
        _browseVm = browseVm;
        _favoritesVm = favoritesVm;
        _settingsVm = settingsVm;
        _currentView = _browseVm;
        ImageViewer = imageViewer;

        // Give BrowseVM a reference to the viewer
        _browseVm.ImageViewer = imageViewer;

        NavigateBrowseCommand = ReactiveCommand.Create(() => { CurrentView = _browseVm; ActiveNav = "browse"; });
        NavigateFavoritesCommand = ReactiveCommand.Create(() => { CurrentView = _favoritesVm; ActiveNav = "favorites"; });
        NavigateSettingsCommand = ReactiveCommand.Create(() => { CurrentView = _settingsVm; ActiveNav = "settings"; });
    }
}
