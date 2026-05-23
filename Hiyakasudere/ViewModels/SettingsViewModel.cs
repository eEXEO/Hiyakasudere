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
    private string _gelbooruApiKey = "";
    private string _gelbooruUserId = "";
    private string _rule34ApiKey = "";
    private string _rule34UserId = "";

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

    public string GelbooruApiKey
    {
        get => _gelbooruApiKey;
        set => this.RaiseAndSetIfChanged(ref _gelbooruApiKey, value);
    }

    public string GelbooruUserId
    {
        get => _gelbooruUserId;
        set => this.RaiseAndSetIfChanged(ref _gelbooruUserId, value);
    }

    public string Rule34ApiKey
    {
        get => _rule34ApiKey;
        set => this.RaiseAndSetIfChanged(ref _rule34ApiKey, value);
    }

    public string Rule34UserId
    {
        get => _rule34UserId;
        set => this.RaiseAndSetIfChanged(ref _rule34UserId, value);
    }

    public ReactiveCommand<Unit, Unit> SaveCommand { get; }

    public string[] SourceNames { get; } = { "Yandere", "Safebooru", "Konachan", "Gelbooru (API key required)", "Rule34 (API key required)" };
    public int[] PostsPerPageOptions { get; } = { 15, 20, 30, 60 };

    public SettingsViewModel(IAppConfigService appConfig)
    {
        _appConfig = appConfig;

        _selectedSource = appConfig.SelectedSource;
        _postsPerPage = appConfig.PostsPerPage;
        _nsfwEnabled = appConfig.IsNSFW;
        _gelbooruApiKey = appConfig.GelbooruApiKey ?? "";
        _gelbooruUserId = appConfig.GelbooruUserId ?? "";
        _rule34ApiKey = appConfig.Rule34ApiKey ?? "";
        _rule34UserId = appConfig.Rule34UserId ?? "";

        SaveCommand = ReactiveCommand.Create(SaveSettings);
    }

    private void SaveSettings()
    {
        _appConfig.UpdateConfig(SelectedSource, PostsPerPage, NsfwEnabled, _appConfig.GetCurrentConfiguration().BlackListedTags);
        _appConfig.GelbooruApiKey = GelbooruApiKey;
        _appConfig.GelbooruUserId = GelbooruUserId;
        _appConfig.Rule34ApiKey = Rule34ApiKey;
        _appConfig.Rule34UserId = Rule34UserId;
        StatusMessage = "Settings saved!";
    }
}
