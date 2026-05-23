using System.Reactive;
using Hiyakasudere.Data.Internal.Config;
using Hiyakasudere.Data.Internal.Data.Post;
using ReactiveUI;

namespace Hiyakasudere.ViewModels;

public class SettingsViewModel : ViewModelBase
{
    private readonly IAppConfigService _appConfig;

    private int _selectedSource;
    private int _postsPerPage;
    private bool _nsfwEnabled;
    private string _statusMessage = "";

    public int SelectedSource
    {
        get => _selectedSource;
        set => this.RaiseAndSetIfChanged(ref _selectedSource, value);
    }

    public int PostsPerPage
    {
        get => _postsPerPage;
        set => this.RaiseAndSetIfChanged(ref _postsPerPage, value);
    }

    public bool NsfwEnabled
    {
        get => _nsfwEnabled;
        set => this.RaiseAndSetIfChanged(ref _nsfwEnabled, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set => this.RaiseAndSetIfChanged(ref _statusMessage, value);
    }

    public ReactiveCommand<Unit, Unit> SaveCommand { get; }

    public string[] SourceNames { get; } = { "Yandere", "Safebooru", "Konachan", "Gelbooru", "Rule34" };
    public int[] PostsPerPageOptions { get; } = { 15, 20, 30, 60 };

    public SettingsViewModel(IAppConfigService appConfig)
    {
        _appConfig = appConfig;

        _selectedSource = appConfig.SelectedSource;
        _postsPerPage = appConfig.PostsPerPage;
        _nsfwEnabled = appConfig.IsNSFW;

        SaveCommand = ReactiveCommand.Create(SaveSettings);
    }

    private void SaveSettings()
    {
        _appConfig.UpdateConfig(SelectedSource, PostsPerPage, NsfwEnabled, _appConfig.GetCurrentConfiguration().BlackListedTags);
        StatusMessage = "Settings saved!";
    }
}
