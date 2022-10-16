namespace Hiyakasudere.Data.ExternalAPI.Yandere
{
    public interface IYanderePostService
    {
        string GenerateRequestURL(int postsPerPage, int currentPage);
        Task<IEnumerable<YanderePost>> GetYandereData(string request);
        Task<int> GetYanderePostCount();
    }
}