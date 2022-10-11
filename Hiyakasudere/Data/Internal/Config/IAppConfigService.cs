using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hiyakasudere.Data.Internal.Config
{
    public interface IAppConfigService
    {
        int postsPerPage { get; }
        bool isNSFW { get; }
    }
}
