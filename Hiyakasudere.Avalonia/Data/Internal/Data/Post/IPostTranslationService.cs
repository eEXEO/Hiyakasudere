namespace Hiyakasudere.Data.Internal.Data.Post
{
    public interface IPostTranslationService
    {
        Task<IEnumerable<PostInternal>> GetPostData(int currentPage);
        void UpdateTags(List<TagInternal> tags);
        Task<int> GetPageCount();
        List<TagInternal> GetTags();

        Task<IEnumerable<TagInternal>> TryAutocompleteTag(string partialTag);
    }
}