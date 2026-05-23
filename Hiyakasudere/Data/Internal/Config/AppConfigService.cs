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
        public string GelbooruApiKey { get; set; } = "";
        public string GelbooruUserId { get; set; } = "";
        public string Rule34ApiKey { get; set; } = "";
        public string Rule34UserId { get; set; } = "";
        public List<TagInternal> BlackListedTags;
        #endregion

        public AppConfigService(IFileManager fileManager)
        {
            this.fileManager = fileManager;
            
            SelectedSource = 1;
            PostsPerPage = 15;
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
                    GelbooruApiKey = fromFile.GelbooruApiKey ?? "";
                    GelbooruUserId = fromFile.GelbooruUserId ?? "";
                    Rule34ApiKey = fromFile.Rule34ApiKey ?? "";
                    Rule34UserId = fromFile.Rule34UserId ?? "";
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
            var config = new ConfigDataModel(SelectedSource, PostsPerPage, IsNSFW, BlackListedTags);
            config.GelbooruApiKey = GelbooruApiKey;
            config.GelbooruUserId = GelbooruUserId;
            config.Rule34ApiKey = Rule34ApiKey;
            config.Rule34UserId = Rule34UserId;
            return config;
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
