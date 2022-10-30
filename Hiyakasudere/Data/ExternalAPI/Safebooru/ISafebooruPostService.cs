namespace Hiyakasudere.Data.ExternalAPI.Safebooru
{
    public interface ISafebooruPostService
    {
        string GenerateRequestURL(int postsPerPage, int currentPage, List<string> tags, List<string> blackTags);
        Task<IEnumerable<SafebooruPost>> GetSafebooruData(string request);
        Task<int> GetSafebooruPostCount(List<string> tags);
    }
}