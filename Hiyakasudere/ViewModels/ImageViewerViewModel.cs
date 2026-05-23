using System;
using System.Collections.Generic;
using System.Reactive;
using System.Threading.Tasks;
using Hiyakasudere.Data.Internal.Functionality.ImageUtils;
using Hiyakasudere.Data.Internal.MultiplatformInterfaces;
using ReactiveUI;

namespace Hiyakasudere.ViewModels;

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
    }

    private async Task SaveCurrentImage()
    {
        if (CurrentPost == null || string.IsNullOrEmpty(CurrentPost.OriginalUrl)) return;

        try
        {
            var base64 = await _imageUtils.GetImageAsBase64(CurrentPost.OriginalUrl);
            await _fileManager.SaveImage(base64, $"Hiyakasudere_{CurrentPost.Id}_{DateTime.Now.ToFileTime()}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Save] {ex.Message}");
        }
    }
}
