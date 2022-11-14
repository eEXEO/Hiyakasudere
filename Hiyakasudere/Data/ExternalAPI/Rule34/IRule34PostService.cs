namespace Hiyakasudere.Data.ExternalAPI.Rule34
{
    public interface IRule34PostService
    {
        string GenerateRequestURL(int postsPerPage, int currentPage, List<string> tags, List<string> blackTags);
        Task<IEnumerable<Rule34Post>> GetRule34Data(string request);
        Task<int> GetRule34PostCount(List<string> tags);
    }
}