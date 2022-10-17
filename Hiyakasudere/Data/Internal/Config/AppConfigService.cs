using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Hiyakasudere.Data.Internal.Config
{
    public class AppConfigService : IAppConfigService
    {
        public AppConfigService()
        {
            SelectedSource = 2;
            PostsPerPage = 18;
            IsNSFW = true;

            SerializeAppConfig();
        }

        protected void SerializeAppConfig()
        {
            string filename = "appConfig.json";
            string filepath = AppContext.BaseDirectory + filename;

            System.Diagnostics.Debug.WriteLine(filepath);

            if (File.Exists(filepath))
            {
                try
                {
                    ConfigDataModel fromFile = JsonConvert.DeserializeObject<ConfigDataModel>(File.ReadAllText(filepath));

                    UpdateConfig(fromFile.SelectedSource, fromFile.PostsPerPage, fromFile.NSFWEnabled);

                }
                catch (Exception e)
                {
                    File.WriteAllText(filepath, JsonConvert.SerializeObject(GetCurrentConfiguration()));
                    System.Diagnostics.Debug.WriteLine(e);
                }

            }
            else
            {
                System.Diagnostics.Debug.WriteLine("FILE NOT EXISTS!");
                File.WriteAllText(filepath, JsonConvert.SerializeObject(GetCurrentConfiguration()));
            }
        }

        protected void UpdateAppConfigFile()
        {
            string filename = "appConfig.json";
            string filepath = AppContext.BaseDirectory + filename;

            try
            {
                File.WriteAllText(filepath, JsonConvert.SerializeObject(GetCurrentConfiguration()));
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }
                
        }

        public ConfigDataModel GetCurrentConfiguration()
        {
            return new ConfigDataModel(SelectedSource, PostsPerPage, IsNSFW);
        }
        public int SelectedSource { get; set; } = 1;
        public int PostsPerPage { get; set; } = 18;
        public bool IsNSFW { get; set; } = false;
        public bool UpdateConfig(int SelectedSource, int PostsPerPage, bool IsNSFW)
        {
            this.IsNSFW = IsNSFW;
            this.SelectedSource = SelectedSource;
            this.PostsPerPage = PostsPerPage;

            System.Diagnostics.Debug.WriteLine("AppConfigService: " + SelectedSource + ", " + PostsPerPage + ", ", IsNSFW);

            UpdateAppConfigFile();

            return true;
        }
    }
}
