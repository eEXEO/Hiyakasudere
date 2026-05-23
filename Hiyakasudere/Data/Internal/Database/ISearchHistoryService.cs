namespace Hiyakasudere.Data.Internal.Database
{
    public interface ISearchHistoryService
    {
        Task AddSearch(string tags, int sourceId);
        Task<List<SearchHistoryEntry>> GetRecentSearches(int limit = 20);
        Task ClearHistory();
    }
}
