using Hiyakasudere.Data.Internal.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hiyakasudere.Data.Internal.MultiplatformInterfaces
{
    public interface IFileManager
    {
        Task<bool> IsConfigFilePresent();
        Task ForceSaveConfigFile(ConfigDataModel filedata);
        Task<ConfigDataModel> ReadConfigFile();
        Task<bool> SaveImage(string base64Image, string filename);

    }
}
