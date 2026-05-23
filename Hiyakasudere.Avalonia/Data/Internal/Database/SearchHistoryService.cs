using Microsoft.EntityFrameworkCore;

namespace Hiyakasudere.Data.Internal.Database
{
    public class SearchHistoryService : ISearchHistoryService
    {
        private readonly AppDbContext _db;

        public SearchHistoryService(AppDbContext db)
        {
            _db = db;
        }

        public async Task AddSearch(string tags, int sourceId)
        {
            if (string.IsNullOrWhiteSpace(tags)) return;

            // Don't add duplicate consecutive searches
            var last = await _db.SearchHistory.OrderByDescending(s => s.SearchedAt).FirstOrDefaultAsync();
            if (last != null && last.Tags == tags && last.SourceId == sourceId) return;

            _db.SearchHistory.Add(new SearchHistoryEntry
            {
                Tags = tags,
                SourceId = sourceId,
                SearchedAt = DateTime.UtcNow
            });
            await _db.SaveChangesAsync();

            // Keep only last 50 entries
            var count = await _db.SearchHistory.CountAsync();
            if (count > 50)
            {
                var toRemove = await _db.SearchHistory.OrderBy(s => s.SearchedAt).Take(count - 50).ToListAsync();
                _db.SearchHistory.RemoveRange(toRemove);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<List<SearchHistoryEntry>> GetRecentSearches(int limit = 20)
        {
            return await _db.SearchHistory
                .OrderByDescending(s => s.SearchedAt)
                .Take(limit)
                .ToListAsync();
        }

        public async Task ClearHistory()
        {
            _db.SearchHistory.RemoveRange(_db.SearchHistory);
            await _db.SaveChangesAsync();
        }
    }
}
