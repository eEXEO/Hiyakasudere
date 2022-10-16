namespace Hiyakasudere.Data.ExternalAPI.Safebooru
{
    public interface ISafebooruPostService
    {
        string GenerateRequestURL(int postsPerPage, int currentPage);
        Task<IEnumerable<SafebooruPost>> GetSafebooruData(string request);
        Task<int> GetSafebooruPostCount();
    }
}