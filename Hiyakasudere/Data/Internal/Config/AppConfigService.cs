using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hiyakasudere.Data.Internal.Config
{
    public class AppConfigService : IAppConfigService
    {
        public int SelectedSource { get; set; } = 1;
        public int PostsPerPage { get; set; } = 20;
        public bool IsNSFW { get; set; } = false;
        public bool UpdateConfig(int SelectedSource, int PostsPerPage, bool IsNSFW)
        {
            this.IsNSFW = IsNSFW;
            this.SelectedSource = SelectedSource;
            this.PostsPerPage = PostsPerPage;

            System.Diagnostics.Debug.WriteLine("AppConfigService: " + SelectedSource + ", " + PostsPerPage + ", ", IsNSFW);

            return true;
        }
    }
}
