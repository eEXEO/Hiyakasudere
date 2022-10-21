using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hiyakasudere.Data.Internal.MultiplatformInterfaces
{
    public interface IDirectoryPicker
    {
        Task<string> PickDirectory();
        string GetPlatformDefaultImageDir();

        string GetFallbackDirectory();
    }
}
