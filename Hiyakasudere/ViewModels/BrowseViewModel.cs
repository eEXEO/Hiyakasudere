using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Hiyakasudere.Data.Internal.Config;
using Hiyakasudere.Data.Internal.Data.Post;
using Hiyakasudere.Data.Internal.Database;
using ReactiveUI;

namespace Hiyakasudere.ViewModels;

public class PostThumbnailViewModel : ViewModelBase
{
    private bool _isFavorited;

    public long Id { get; set; }
    public string PreviewUrl { get; set; } = "";
    public string SampleUrl { get; set; } = "";
    public string OriginalUrl { get; set; } = "";
    public string Tags { get; set; } = "";
    public string Rating { get; set; } = "";
    public long Score { get; set; }
    public long Width { get; set; }
    public long Height { get; set; }
    public int SourceId { get; set; }

    /// <summary>Preview image width from API</summary>
    public long PreviewWidth { get; set; }
    /// <summary>Preview image height from API</summary>
    public long PreviewHeight { get; set; }

    /// <summary>Calculated display height based on fixed column width (220px) and aspect ratio</summary>
    public double DisplayHeight
    {
        get
        {
            const double columnWidth = 220.0;
            if (PreviewWidth > 0 && PreviewHeight > 0)
                return columnWidth * PreviewHeight / PreviewWidth;
            if (Width > 0 && Height > 0)
                return columnWidth * Height / Width;
            return columnWidth; // Square fallback
        }
    }

    public bool IsFavorited
    {
        get => _isFavorited;
        set => this.RaiseAndSetIfChanged(ref _isFavorited, value);
    }

    public string HeartIcon => IsFavorited ? "\u2764" : "\u2661";
}

public class BrowseViewModel : ViewModelBase
{
    private readonly IPostTranslationService _postService;
    private readonly IAppConfigService _appConfig;
    private readonly IFavoritesService _favoritesService;
    private readonly ISearchHistoryService _searchHistoryService;

    private string _searchText = "";
    private int _currentPage = 1;
    private int _totalPages = 1;
    private bool _isLoading = false;
    private bool _showSkeleton = true;
    private string _errorMessage;
    private string _statusText = "Ready";

    public ObservableCollection<PostThumbnailViewModel> Posts { get; } = new();
    public ObservableCollection<string> RecentSearches { get; } = new();

    public string SearchText
    {
        get => _searchText;
        set => this.RaiseAndSetIfChanged(ref _searchText, value);
    }

    public int CurrentPage
    {
        get => _currentPage;
        set => this.RaiseAndSetIfChanged(ref _currentPage, value);
    }

    public int TotalPages
    {
        get => _totalPages;
        set => this.RaiseAndSetIfChanged(ref _totalPages, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => this.RaiseAndSetIfChanged(ref _isLoading, value);
    }

    public bool ShowSkeleton
    {
        get => _showSkeleton;
        set => this.RaiseAndSetIfChanged(ref _showSkeleton, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
    }

    public string StatusText
    {
        get => _statusText;
        set => this.RaiseAndSetIfChanged(ref _statusText, value);
    }

    public ReactiveCommand<Unit, Unit> SearchCommand { get; }
    public ReactiveCommand<Unit, Unit> NextPageCommand { get; }
    public ReactiveCommand<Unit, Unit> PrevPageCommand { get; }
    public ReactiveCommand<Unit, Unit> LoadNextPageCommand { get; }
    public ReactiveCommand<PostThumbnailViewModel, Unit> ToggleFavoriteCommand { get; }
    public ReactiveCommand<PostThumbnailViewModel, Unit> OpenImageCommand { get; }

    public ImageViewerViewModel ImageViewer { get; set; }

    public BrowseViewModel(IPostTranslationService postService, IAppConfigService appConfig,
        IFavoritesService favoritesService, ISearchHistoryService searchHistoryService)
    {
        _postService = postService;
        _appConfig = appConfig;
        _favoritesService = favoritesService;
        _searchHistoryService = searchHistoryService;

        SearchCommand = ReactiveCommand.CreateFromTask(ExecuteSearch);
        NextPageCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            if (CurrentPage < TotalPages) { CurrentPage++; await LoadPage(); }
        });
        PrevPageCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            if (CurrentPage > 1) { CurrentPage--; Posts.Clear(); await LoadPage(); }
        });
        LoadNextPageCommand = ReactiveCommand.CreateFromTask(LoadNextPage);
        ToggleFavoriteCommand = ReactiveCommand.CreateFromTask<PostThumbnailViewModel>(ToggleFavorite);
        OpenImageCommand = ReactiveCommand.Create<PostThumbnailViewModel>(OpenImage);

        // Initial load
        Task.Run(async () =>
        {
            await LoadRecentSearches();
            await LoadPage();
        });
    }

    private async Task ExecuteSearch()
    {
        var tags = SearchText?.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList() ?? new();
        var tagInternals = tags.Select(t => new TagInternal(0, t, 1, 1, false)).ToList();
        _postService.UpdateTags(tagInternals);
        CurrentPage = 1;

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            await _searchHistoryService.AddSearch(SearchText, _appConfig.SelectedSource);
            await LoadRecentSearches();
        }

        await LoadPage();
    }

    private async Task LoadPage()
    {
        IsLoading = true;
        ErrorMessage = null;
        Posts.Clear();
        ShowSkeleton = true;

        try
        {
            TotalPages = await _postService.GetPageCount();
            var posts = await _postService.GetPostData(CurrentPage);

            if (posts == null || !posts.Any())
            {
                ErrorMessage = "No posts found. Try different tags.";
                StatusText = "No results";
                ShowSkeleton = false;
                return;
            }

            foreach (var post in posts)
            {
                var vm = new PostThumbnailViewModel
                {
                    Id = post.Id,
                    PreviewUrl = post.PreviewUrl?.AbsoluteUri ?? "",
                    SampleUrl = post.SampleUrl?.AbsoluteUri ?? "",
                    OriginalUrl = post.OriginalUrl?.AbsoluteUri ?? "",
                    Tags = post.Tags ?? "",
                    Rating = post.Rating ?? "",
                    Score = post.Score,
                    Width = post.OriginalWidth,
                    Height = post.OriginalHeight,
                    PreviewWidth = post.PreviewWidth,
                    PreviewHeight = post.PreviewHeight,
                    SourceId = _appConfig.SelectedSource,
                    IsFavorited = await _favoritesService.IsFavorite(post.Id, _appConfig.SelectedSource)
                };
                Posts.Add(vm);
            }

            StatusText = $"Page {CurrentPage}/{TotalPages} - {Posts.Count} posts";
        }
        catch (System.Net.Http.HttpRequestException)
        {
            ErrorMessage = "Network error. Check your internet connection.";
            StatusText = "Connection failed";
        }
        catch (Exception ex)
        {
            ErrorMessage = "Failed to load posts. Please try again.";
            StatusText = "Error";
            System.Diagnostics.Debug.WriteLine(ex);
        }
        finally
        {
            IsLoading = false;
            ShowSkeleton = false;
        }
    }

    private async Task LoadNextPage()
    {
        if (IsLoading || CurrentPage >= TotalPages) return;

        CurrentPage++;
        IsLoading = true;

        try
        {
            var posts = await _postService.GetPostData(CurrentPage);

            if (posts != null && posts.Any())
            {
                foreach (var post in posts)
                {
                    var vm = new PostThumbnailViewModel
                    {
                        Id = post.Id,
                        PreviewUrl = post.PreviewUrl?.AbsoluteUri ?? "",
                        SampleUrl = post.SampleUrl?.AbsoluteUri ?? "",
                        OriginalUrl = post.OriginalUrl?.AbsoluteUri ?? "",
                        Tags = post.Tags ?? "",
                        Rating = post.Rating ?? "",
                        Score = post.Score,
                        Width = post.OriginalWidth,
                        Height = post.OriginalHeight,
                        PreviewWidth = post.PreviewWidth,
                        PreviewHeight = post.PreviewHeight,
                        SourceId = _appConfig.SelectedSource,
                        IsFavorited = await _favoritesService.IsFavorite(post.Id, _appConfig.SelectedSource)
                    };
                    Posts.Add(vm);
                }
            }

            StatusText = $"Page {CurrentPage}/{TotalPages} - {Posts.Count} posts loaded";
        }
        catch (Exception ex)
        {
            CurrentPage--; // Revert on failure
            System.Diagnostics.Debug.WriteLine(ex);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task ToggleFavorite(PostThumbnailViewModel post)
    {
        if (post.IsFavorited)
        {
            await _favoritesService.RemoveFavorite(post.Id, post.SourceId);
            post.IsFavorited = false;
        }
        else
        {
            await _favoritesService.AddFavorite(new FavoritePost
            {
                SourcePostId = post.Id,
                SourceId = post.SourceId,
                PreviewUrl = post.PreviewUrl,
                SampleUrl = post.SampleUrl,
                OriginalUrl = post.OriginalUrl,
                Tags = post.Tags,
                Rating = post.Rating,
                Score = post.Score,
                Width = post.Width,
                Height = post.Height
            });
            post.IsFavorited = true;
        }
    }

    private async Task LoadRecentSearches()
    {
        var searches = await _searchHistoryService.GetRecentSearches(6);
        RecentSearches.Clear();
        foreach (var s in searches)
            RecentSearches.Add(s.Tags);
    }

    private void OpenImage(PostThumbnailViewModel post)
    {
        if (ImageViewer == null) return;
        var index = Posts.IndexOf(post);
        ImageViewer.Open(Posts.ToList(), index >= 0 ? index : 0);
    }
}
