namespace Hiyakasudere.Data.Internal.Data.Post
{
    public interface IPostTranslationService
    {
        Task<IEnumerable<PostInternal>> GetPostData(int currentPage);
        void UpdateTags(string tags);
        Task<int> GetPageCount();
        List<string> GetTags();
    }
}