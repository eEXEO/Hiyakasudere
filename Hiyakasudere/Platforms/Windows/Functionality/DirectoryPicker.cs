using Hiyakasudere.Data.Internal.MultiplatformInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WDP = Windows.Storage.Pickers.FolderPicker;

namespace Hiyakasudere.Platforms.Windows.Functionality
{
    internal class DirectoryPicker : IDirectoryPicker
    {
        public string GetPlatformDefaultImageDir()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
        }

        public string GetFallbackDirectory()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        }

        public async Task<string> PickDirectory()
        {
            var dirPicker = new WDP();
            var handle = ((MauiWinUIWindow)App.Current.Windows[0].Handler.PlatformView).WindowHandle;

            WinRT.Interop.InitializeWithWindow.Initialize(dirPicker, handle);

            var result = await dirPicker.PickSingleFolderAsync();

            if(result is null)
            {
                return GetPlatformDefaultImageDir();
            }else return result.Path;
        }
    }
}
