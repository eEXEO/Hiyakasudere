using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hiyakasudere.Data.Internal.Config
{
    public class AppConfigService : IAppConfigService
    {
        public int postsPerPage { get; } = 20;
        public bool isNSFW { get; } = false;
    }
}
