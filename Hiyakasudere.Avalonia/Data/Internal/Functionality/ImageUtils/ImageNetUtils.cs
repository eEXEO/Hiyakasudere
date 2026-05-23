using Hiyakasudere.Data.ExternalAPI.Yandere;
using Hiyakasudere.Data.Internal.Config;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Hiyakasudere.Data.Internal.Functionality.ImageUtils
{
    public class ImageNetUtils : IImageNetUtils
    {
        HttpClient client;

        public ImageNetUtils()
        {
            client = new HttpClient();
        }

        public async Task<string> GetImageAsBase64(string request)
        {
            var bytes = await client.GetByteArrayAsync(request);

            return Convert.ToBase64String(bytes);
        }
    }
}
