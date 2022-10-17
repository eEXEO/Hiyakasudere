using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hiyakasudere.Data.Internal.Config
{
    public interface IAppConfigService
    {
        public ConfigDataModel GetCurrentConfiguration();
        int SelectedSource { get; set; }
        int PostsPerPage { get; set; }
        bool IsNSFW { get; set; }
        bool UpdateConfig(int SelectedSource, int PostsPerPage, bool IsNSFW);
    }
}
