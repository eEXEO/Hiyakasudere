using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Reactive;
using System.Threading.Tasks;
using Hiyakasudere.Data.Internal.Config;
using ReactiveUI;

namespace Hiyakasudere.ViewModels;

public class SourceStatusViewModel : ViewModelBase
{
    private string _status = "checking"; // "online", "offline", "checking"

    public string Name { get; set; } = "";
    public string Url { get; set; } = "";

    public string Status
    {
        get => _status;
        set => this.RaiseAndSetIfChanged(ref _status, value);
    }

    public string StatusColor => Status switch
    {
        "online" => "#4caf50",
        "offline" => "#f44336",
        _ => "#ff9800"
    };

    public string StatusText => Status switch
    {
        "online" => "Reachable",
        "offline" => "Unreachable",
        _ => "Checking..."
    };
}

public class SettingsViewModel : ViewModelBase
{
    private readonly IAppConfigService _appConfig;
    private static readonly HttpClient _httpClient = new()
    {
        Timeout = TimeSpan.FromSeconds(8)
    };

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

    public ObservableCollection<SourceStatusViewModel> SourceStatuses { get; } = new();

    public ReactiveCommand<Unit, Unit> SaveCommand { get; }
    public ReactiveCommand<Unit, Unit> CheckSourcesCommand { get; }

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
        CheckSourcesCommand = ReactiveCommand.CreateFromTask(CheckSourceStatuses);

        // Initialize source statuses
        InitializeSourceStatuses();

        // Auto-check on startup
        Task.Run(CheckSourceStatuses);
    }

    private void InitializeSourceStatuses()
    {
        SourceStatuses.Add(new SourceStatusViewModel { Name = "Yandere", Url = "https://yande.re" });
        SourceStatuses.Add(new SourceStatusViewModel { Name = "Safebooru", Url = "https://safebooru.org" });
        SourceStatuses.Add(new SourceStatusViewModel { Name = "Konachan", Url = "https://konachan.com" });
        SourceStatuses.Add(new SourceStatusViewModel { Name = "Gelbooru", Url = "https://gelbooru.com" });
        SourceStatuses.Add(new SourceStatusViewModel { Name = "Rule34", Url = "https://api.rule34.xxx" });
    }

    private async Task CheckSourceStatuses()
    {
        foreach (var source in SourceStatuses)
        {
            source.Status = "checking";
        }

        foreach (var source in SourceStatuses)
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Head, source.Url);
                var response = await _httpClient.SendAsync(request);
                source.Status = response.IsSuccessStatusCode ? "online" : "online"; // Even non-200 means reachable
            }
            catch
            {
                source.Status = "offline";
            }

            // Notify property changes for computed properties
            source.RaisePropertyChanged(nameof(SourceStatusViewModel.StatusColor));
            source.RaisePropertyChanged(nameof(SourceStatusViewModel.StatusText));
        }
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
