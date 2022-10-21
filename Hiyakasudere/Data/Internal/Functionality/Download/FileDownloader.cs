using Hiyakasudere.Data.Internal.Config;
using Hiyakasudere.Data.Internal.MultiplatformInterfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Hiyakasudere.Data.Internal.Functionality.Download
{

    public class FileDownloader
    {
        private readonly IAppConfigService config;

        public FileDownloader(IAppConfigService config)
        {
            this.config = config;
        }

        public async Task DownloadFromUrl(string Url)
        {
            HttpClient client = new HttpClient();
            var responseStream = await client.GetStreamAsync(Url);

            try
            {
                using var fileStream = new FileStream(config.ImageSavePath, FileMode.Create);
                responseStream.CopyTo(fileStream);
            }catch(Exception)
            {
                System.Diagnostics.Debug.WriteLine("FileDownloader: Access to " + config.ImageSavePath + " was denied! Using fallback path");
                using var fileStream = new FileStream(config.PassFallbackSaveDir(), FileMode.Create);
                responseStream.CopyTo(fileStream);
            }

        }
    }
}
