using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hiyakasudere.Data.Internal.Data.Post;

namespace Hiyakasudere.Data.Internal.Config
{
    public interface IAppConfigService
    {
        public ConfigDataModel GetCurrentConfiguration();
        public List<string> GetSimplifiedBlackTags();
        bool IsLoaded { get; set; }
        int SelectedSource { get; set; }
        int PostsPerPage { get; set; }
        bool IsNSFW { get; set; }
        string ImageSavePath { get; set; }
        bool UpdateConfig(int SelectedSource, int PostsPerPage, bool IsNSFW, List<TagInternal> BlaclistedTags);
    }
}
