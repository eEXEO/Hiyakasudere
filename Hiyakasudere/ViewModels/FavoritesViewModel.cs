using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;
using Hiyakasudere.Data.Internal.Database;
using ReactiveUI;

namespace Hiyakasudere.ViewModels;

public class FavoritesViewModel : ViewModelBase
{
    private readonly IFavoritesService _favoritesService;
    private int _count;

    public ObservableCollection<FavoritePost> Favorites { get; } = new();

    public int Count
    {
        get => _count;
        set => this.RaiseAndSetIfChanged(ref _count, value);
    }

    public ReactiveCommand<FavoritePost, Unit> RemoveFavoriteCommand { get; }
    public ReactiveCommand<Unit, Unit> RefreshCommand { get; }

    public FavoritesViewModel(IFavoritesService favoritesService)
    {
        _favoritesService = favoritesService;

        RemoveFavoriteCommand = ReactiveCommand.CreateFromTask<FavoritePost>(RemoveFavorite);
        RefreshCommand = ReactiveCommand.CreateFromTask(LoadFavorites);

        Task.Run(LoadFavorites);
    }

    public async Task LoadFavorites()
    {
        var favs = await _favoritesService.GetAllFavorites();
        Favorites.Clear();
        foreach (var f in favs)
            Favorites.Add(f);
        Count = Favorites.Count;
    }

    private async Task RemoveFavorite(FavoritePost fav)
    {
        await _favoritesService.RemoveFavorite(fav.SourcePostId, fav.SourceId);
        Favorites.Remove(fav);
        Count = Favorites.Count;
    }
}
