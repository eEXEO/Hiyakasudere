namespace Hiyakasudere.Data.ExternalAPI.Konachan
{
    public interface IKonachanPostService
    {
        string GenerateRequestURL(int postsPerPage, int currentPage, List<string> tags, List<string> blackTags);
        Task<IEnumerable<KonachanPost>> GetKonachanData(string request);
        Task<int> GetKonachanPostCount(List<string> tags);
        Task<IEnumerable<KonachanTag>> GetTagsAutocompletion(string partialTag);
    }
}