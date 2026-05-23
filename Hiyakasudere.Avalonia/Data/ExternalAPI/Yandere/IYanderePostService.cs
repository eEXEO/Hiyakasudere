namespace Hiyakasudere.Data.ExternalAPI.Yandere
{
    public interface IYanderePostService
    {
        string GenerateRequestURL(int postsPerPage, int currentPage, List<string> tags, List<string> blackTags);
        Task<IEnumerable<YanderePost>> GetYandereData(string request);
        Task<int> GetYanderePostCount(List<string> tags);
        Task<IEnumerable<YandereTag>> GetTagsAutocompletion(string partialTag);
    }
}