namespace Hiyakasudere.Data.Internal.Database
{
    public interface IFavoritesService
    {
        Task<bool> IsFavorite(long sourcePostId, int sourceId);
        Task AddFavorite(FavoritePost post);
        Task RemoveFavorite(long sourcePostId, int sourceId);
        Task<List<FavoritePost>> GetAllFavorites(int? sourceFilter = null);
        Task<int> GetFavoritesCount();
    }
}
