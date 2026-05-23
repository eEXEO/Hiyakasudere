using Microsoft.EntityFrameworkCore;

namespace Hiyakasudere.Data.Internal.Database
{
    public class FavoritesService : IFavoritesService
    {
        private readonly AppDbContext _db;

        public FavoritesService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<bool> IsFavorite(long sourcePostId, int sourceId)
        {
            return await _db.Favorites.AnyAsync(f => f.SourcePostId == sourcePostId && f.SourceId == sourceId);
        }

        public async Task AddFavorite(FavoritePost post)
        {
            var existing = await _db.Favorites.FirstOrDefaultAsync(f => f.SourcePostId == post.SourcePostId && f.SourceId == post.SourceId);
            if (existing == null)
            {
                post.AddedAt = DateTime.UtcNow;
                _db.Favorites.Add(post);
                await _db.SaveChangesAsync();
            }
        }

        public async Task RemoveFavorite(long sourcePostId, int sourceId)
        {
            var existing = await _db.Favorites.FirstOrDefaultAsync(f => f.SourcePostId == sourcePostId && f.SourceId == sourceId);
            if (existing != null)
            {
                _db.Favorites.Remove(existing);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<List<FavoritePost>> GetAllFavorites(int? sourceFilter = null)
        {
            var query = _db.Favorites.AsQueryable();
            if (sourceFilter.HasValue)
                query = query.Where(f => f.SourceId == sourceFilter.Value);

            return await query.OrderByDescending(f => f.AddedAt).ToListAsync();
        }

        public async Task<int> GetFavoritesCount()
        {
            return await _db.Favorites.CountAsync();
        }
    }
}
