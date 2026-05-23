using Hiyakasudere.Data.Internal.Data.Post;

namespace Hiyakasudere.Data.ExternalAPI.Gelbooru
{
    public interface IGelbooruPostService
    {
        string GenerateRequestURL(int postsPerPage, int currentPage, List<string> tags, List<string> blackTags);
        Task<IEnumerable<GelbooruPost>> GetGelbooruData(string request);
        Task<int> GetGelbooruPostCount(List<string> tags);
        Task<IEnumerable<TagInternal>> GetTagsAutocompletion(string partialTag);
    }
}
