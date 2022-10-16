namespace Hiyakasudere.Data.Internal.Data.Post
{
    public interface IPostTranslationService
    {
        Task<IEnumerable<PostInternal>> GetPostData(int currentPage);
    }
}