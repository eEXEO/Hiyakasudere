using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Hiyakasudere.Data.Internal.Functionality.ImageUtils;
using Hiyakasudere.Data.Internal.MultiplatformInterfaces;
using ReactiveUI;

namespace Hiyakasudere.ViewModels;

public class TagViewModel : ViewModelBase
{
    public string Name { get; set; } = "";
    public string Color { get; set; } = "#4a9eff"; // Default blue for general
    public string Background { get; set; } = "#1a3a5c";
}

public class ImageViewerViewModel : ViewModelBase
{
    private readonly IImageNetUtils _imageUtils;
    private readonly IFileManager _fileManager;
    private List<PostThumbnailViewModel> _posts = new();
    private int _currentIndex = 0;
    private bool _isOpen;
    private string _imageUrl = "";
    private string _dimensionsText = "";
    private PostThumbnailViewModel _currentPost;

    public event EventHandler<string> ImageSaved;

    public ObservableCollection<TagViewModel> CurrentTags { get; } = new();

    public bool IsOpen
    {
        get => _isOpen;
        set => this.RaiseAndSetIfChanged(ref _isOpen, value);
    }

    public string ImageUrl
    {
        get => _imageUrl;
        set => this.RaiseAndSetIfChanged(ref _imageUrl, value);
    }

    public string DimensionsText
    {
        get => _dimensionsText;
        set => this.RaiseAndSetIfChanged(ref _dimensionsText, value);
    }

    public PostThumbnailViewModel CurrentPost
    {
        get => _currentPost;
        set => this.RaiseAndSetIfChanged(ref _currentPost, value);
    }

    public ReactiveCommand<Unit, Unit> CloseCommand { get; }
    public ReactiveCommand<Unit, Unit> NextCommand { get; }
    public ReactiveCommand<Unit, Unit> PrevCommand { get; }
    public ReactiveCommand<Unit, Unit> SaveCommand { get; }

    public ImageViewerViewModel(IImageNetUtils imageUtils, IFileManager fileManager)
    {
        _imageUtils = imageUtils;
        _fileManager = fileManager;

        CloseCommand = ReactiveCommand.Create(() => { IsOpen = false; });
        NextCommand = ReactiveCommand.Create(NavigateNext);
        PrevCommand = ReactiveCommand.Create(NavigatePrev);
        SaveCommand = ReactiveCommand.CreateFromTask(SaveCurrentImage);
    }

    public void Open(List<PostThumbnailViewModel> posts, int index)
    {
        _posts = posts;
        _currentIndex = Math.Clamp(index, 0, posts.Count - 1);
        UpdateCurrent();
        IsOpen = true;
    }

    private void NavigateNext()
    {
        if (_currentIndex < _posts.Count - 1)
        {
            _currentIndex++;
            UpdateCurrent();
        }
    }

    private void NavigatePrev()
    {
        if (_currentIndex > 0)
        {
            _currentIndex--;
            UpdateCurrent();
        }
    }

    private void UpdateCurrent()
    {
        if (_posts.Count == 0) return;
        CurrentPost = _posts[_currentIndex];
        // Use sample URL for viewing (higher quality than preview)
        ImageUrl = !string.IsNullOrEmpty(CurrentPost.SampleUrl) ? CurrentPost.SampleUrl : CurrentPost.PreviewUrl;
        DimensionsText = $"{CurrentPost.Width}x{CurrentPost.Height}";
        UpdateTags();
    }

    private void UpdateTags()
    {
        CurrentTags.Clear();
        if (CurrentPost == null || string.IsNullOrEmpty(CurrentPost.Tags)) return;

        var tagStrings = CurrentPost.Tags.Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Take(30); // Limit to 30 tags for UI performance

        foreach (var tag in tagStrings)
        {
            var (color, bg) = GetTagColor(tag);
            CurrentTags.Add(new TagViewModel
            {
                Name = tag,
                Color = color,
                Background = bg
            });
        }
    }

    /// <summary>
    /// Determines tag color based on common tag patterns.
    /// Artist tags usually have underscores and are specific names.
    /// Character tags often end with series names in parens.
    /// Copyright tags are series/franchise names.
    /// Meta tags are things like highres, absurdres, etc.
    /// </summary>
    private (string color, string bg) GetTagColor(string tag)
    {
        // Meta tags (common known ones)
        var metaTags = new HashSet<string> { "highres", "absurdres", "incredibly_absurdres", 
            "commentary", "translated", "commentary_request", "translation_request",
            "tagme", "bad_id", "bad_pixiv_id", "duplicate", "sample", "resized",
            "wallpaper", "scan", "screencap", "vector", "monochrome", "grayscale",
            "comic", "4koma", "manga", "doujinshi", "cover", "album_cover" };
        
        if (metaTags.Contains(tag))
            return ("#ff9800", "#2d1f00"); // Orange for meta

        // Rating-like tags
        if (tag.StartsWith("rating:") || tag == "safe" || tag == "questionable" || tag == "explicit")
            return ("#ff9800", "#2d1f00"); // Orange for meta

        // Common character indicators (contains parentheses for series name)
        if (tag.Contains("_(") && tag.EndsWith(")"))
            return ("#4caf50", "#0d2a0d"); // Green for character

        // Artist indicators (if someone uses "drawn_by_" or if it looks like a username)
        if (tag.StartsWith("drawn_by_") || tag.StartsWith("artist:"))
            return ("#f44336", "#2d0a0a"); // Red for artist

        // Copyright/series common patterns
        var copyrightTags = new HashSet<string> { "original", "touhou", "kantai_collection", 
            "fate/grand_order", "fate_(series)", "vocaloid", "genshin_impact", "hololive",
            "blue_archive", "arknights", "azur_lane", "idolmaster", "love_live!",
            "naruto", "one_piece", "pokemon", "sword_art_online" };

        if (copyrightTags.Contains(tag))
            return ("#9c27b0", "#1a0a2e"); // Purple for copyright

        // Default: general tag
        return ("#4a9eff", "#0a1a2e"); // Blue for general
    }

    private async Task SaveCurrentImage()
    {
        if (CurrentPost == null || string.IsNullOrEmpty(CurrentPost.OriginalUrl)) return;

        try
        {
            var base64 = await _imageUtils.GetImageAsBase64(CurrentPost.OriginalUrl);
            await _fileManager.SaveImage(base64, $"Hiyakasudere_{CurrentPost.Id}_{DateTime.Now.ToFileTime()}");
            ImageSaved?.Invoke(this, "Image saved successfully!");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Save] {ex.Message}");
            ImageSaved?.Invoke(this, "Failed to save image.");
        }
    }
}
