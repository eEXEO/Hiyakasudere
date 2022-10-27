using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hiyakasudere.Data.Internal.MultiplatformInterfaces;
using Newtonsoft.Json;

namespace Hiyakasudere.Data.Internal.Config
{
    public class AppConfigService : IAppConfigService
    {
        #region VARIABLES

        readonly IFileManager fileManager;

        public bool IsLoaded { get; set; } = false;

        public int SelectedSource { get; set; } = 1;
        public int PostsPerPage { get; set; } = 18;
        public bool IsNSFW { get; set; } = false;
        public string ImageSavePath { get; set; } = "";

        #endregion

        public AppConfigService(IFileManager fileManager)
        {
            this.fileManager = fileManager;
            
            SelectedSource = 1;
            PostsPerPage = 18;
            IsNSFW = false;
            ImageSavePath = "";

            SerializeAppConfig().WaitAsync(CancellationToken.None);
        }

        protected async Task SerializeAppConfig()
        {
            try
            {
                if (await fileManager.IsConfigFilePresent())
                {
                    var fromFile = await fileManager.ReadConfigFile();
                    UpdateConfig(fromFile.SelectedSource, fromFile.PostsPerPage, fromFile.NSFWEnabled, fromFile.ImageSavePath);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("FILE NOT EXISTS!");
                    await fileManager.ForceSaveConfigFile(GetCurrentConfiguration());
                }
            }catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }finally
            {
                IsLoaded = true;
            }
        }

        protected void UpdateAppConfigFile()
        {
            fileManager.ForceSaveConfigFile(GetCurrentConfiguration()).WaitAsync(CancellationToken.None);
        }

        public ConfigDataModel GetCurrentConfiguration()
        {
            return new ConfigDataModel(SelectedSource, PostsPerPage, IsNSFW, ImageSavePath);
        }

        public bool UpdateConfig(int SelectedSource, int PostsPerPage, bool IsNSFW)
        {
            this.IsNSFW = IsNSFW;
            this.SelectedSource = SelectedSource;
            this.PostsPerPage = PostsPerPage;

            System.Diagnostics.Debug.WriteLine("AppConfigService: " + SelectedSource + ", " + PostsPerPage + ", ", IsNSFW);

            UpdateAppConfigFile();

            return true;
        }

        public bool UpdateConfig(int SelectedSource, int PostsPerPage, bool IsNSFW, string ImageSavePath)
        {
            this.IsNSFW = IsNSFW;
            this.SelectedSource = SelectedSource;
            this.PostsPerPage = PostsPerPage;
            this.ImageSavePath = ImageSavePath;

            System.Diagnostics.Debug.WriteLine("AppConfigService: " + SelectedSource + ", " + PostsPerPage + ", ", IsNSFW);

            UpdateAppConfigFile();

            return true;
        }
    }
}
