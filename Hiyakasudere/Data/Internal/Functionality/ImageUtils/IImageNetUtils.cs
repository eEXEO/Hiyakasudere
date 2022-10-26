namespace Hiyakasudere.Data.Internal.Functionality.ImageUtils
{
    public interface IImageNetUtils
    {
        Task<string> GetImageAsBase64(string request);
    }
}