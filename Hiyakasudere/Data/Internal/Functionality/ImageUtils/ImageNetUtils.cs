namespace Hiyakasudere.Data.Internal.Functionality.ImageUtils
{
    public class ImageNetUtils : IImageNetUtils
    {
        public async Task<string> GetImageAsBase64(string request)
        {
            var bytes = await HttpClientProvider.Client.GetByteArrayAsync(request);
            return Convert.ToBase64String(bytes);
        }
    }
}
