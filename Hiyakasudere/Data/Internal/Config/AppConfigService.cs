using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hiyakasudere.Data.Internal.Data.Post;
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
        public List<TagInternal> BlackListedTags;
        #endregion

        public AppConfigService(IFileManager fileManager)
        {
            this.fileManager = fileManager;
            
            SelectedSource = 1;
            PostsPerPage = 18;
            IsNSFW = false;
            ImageSavePath = "";
            BlackListedTags = new();
            //BlackListedTags.Add(new TagInternal(0, "", 1, 1, true));

            SerializeAppConfig().WaitAsync(CancellationToken.None);
        }

        public List<string> GetSimplifiedBlackTags()
        {
            List<string> temp = new();
            try
            {
                foreach (TagInternal tag in BlackListedTags)
                {
                    temp.Add(tag.Name);
                }
            }
            catch (Exception)
            {

            }

            return temp;
        }

        protected async Task SerializeAppConfig()
        {
            try
            {
                if (await fileManager.IsConfigFilePresent())
                {
                    var fromFile = await fileManager.ReadConfigFile();
                    UpdateConfig(fromFile.SelectedSource, fromFile.PostsPerPage, fromFile.NSFWEnabled, fromFile.BlackListedTags);
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
            return new ConfigDataModel(SelectedSource, PostsPerPage, IsNSFW, BlackListedTags);
        }

        public bool UpdateConfig(int SelectedSource, int PostsPerPage, bool IsNSFW, List<TagInternal> BlackListedTags)
        {
            this.IsNSFW = IsNSFW;
            this.SelectedSource = SelectedSource;
            this.PostsPerPage = PostsPerPage;
            this.BlackListedTags = BlackListedTags;

            System.Diagnostics.Debug.WriteLine("AppConfigService: " + SelectedSource + ", " + PostsPerPage + ", ", IsNSFW);

            UpdateAppConfigFile();

            return true;
        }
    }
}
