using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using ReactiveUI;

namespace Hiyakasudere.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private ViewModelBase _currentView;
    private string _activeNav = "browse";
    private bool _isToastVisible;
    private string _toastMessage = "";
    private CancellationTokenSource _toastCts;

    public ViewModelBase CurrentView
    {
        get => _currentView;
        set => this.RaiseAndSetIfChanged(ref _currentView, value);
    }

    public string ActiveNav
    {
        get => _activeNav;
        set
        {
            this.RaiseAndSetIfChanged(ref _activeNav, value);
            this.RaisePropertyChanged(nameof(IsBrowseActive));
            this.RaisePropertyChanged(nameof(IsFavoritesActive));
            this.RaisePropertyChanged(nameof(IsSettingsActive));
        }
    }

    public bool IsBrowseActive => ActiveNav == "browse";
    public bool IsFavoritesActive => ActiveNav == "favorites";
    public bool IsSettingsActive => ActiveNav == "settings";

    public bool IsToastVisible
    {
        get => _isToastVisible;
        set => this.RaiseAndSetIfChanged(ref _isToastVisible, value);
    }

    public string ToastMessage
    {
        get => _toastMessage;
        set => this.RaiseAndSetIfChanged(ref _toastMessage, value);
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

        // Wire up save confirmation toast from ImageViewer
        imageViewer.ImageSaved += OnImageSaved;

        NavigateBrowseCommand = ReactiveCommand.Create(() => { CurrentView = _browseVm; ActiveNav = "browse"; });
        NavigateFavoritesCommand = ReactiveCommand.Create(() => { CurrentView = _favoritesVm; ActiveNav = "favorites"; });
        NavigateSettingsCommand = ReactiveCommand.Create(() => { CurrentView = _settingsVm; ActiveNav = "settings"; });
    }

    private void OnImageSaved(object sender, string message)
    {
        ShowToast(message);
    }

    public async void ShowToast(string message)
    {
        // Cancel previous toast if any
        _toastCts?.Cancel();
        _toastCts = new CancellationTokenSource();
        var token = _toastCts.Token;

        ToastMessage = message;
        IsToastVisible = true;

        try
        {
            await Task.Delay(3000, token);
            IsToastVisible = false;
        }
        catch (TaskCanceledException)
        {
            // Toast was replaced by a new one
        }
    }
}
