using Hiyakasudere.Data.Internal.MultiplatformInterfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Hiyakasudere.Data.Internal.Config;
using Newtonsoft.Json;
using Windows.Storage.Streams;
using static System.Net.Mime.MediaTypeNames;
using System.Runtime.InteropServices.WindowsRuntime;
using System.IO;
using Windows.Networking.Sockets;
using System.Collections;

namespace Hiyakasudere.Platforms.Windows.Functionality
{
    internal class FileManager : IFileManager
    {
        #region PATHS DEFINE
        protected string GetRoamingDirPath()
        {
            return ApplicationData.Current.RoamingFolder.Path;
        }

        protected async Task<StorageFolder> GetAppMainDir()
        {
            StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(GetRoamingDirPath());
            return folder;
        }

        /// <summary>Function <c>GetAppImagesDir</c> returns user's StorageFolder of .../Pictures/Hiyakasudere/ 
        /// It will create new folder if needed.
        /// Works only on Windows 10.0.19041.0 and newer</summary>
        ///
        private async Task<StorageFolder> GetAppImagesDir()
        {
            StorageFolder folder = await KnownFolders.GetFolderAsync(KnownFolderId.PicturesLibrary);

            try
            {
                await folder.CreateFolderAsync("Hiyakasudere");
            }
            catch (Exception) { }

            folder = await folder.GetFolderAsync("Hiyakasudere");
            return folder;
        }
        #endregion

        #region CONFIG FILE FUNCIONALITY
        /// <summary>Function <c>IsConfigFilePresent</c> checks if appConfig.json is present in app install directory. Then returns boolean value</summary>
        ///
        public async Task<bool> IsConfigFilePresent()
        {
            try
            {
                StorageFile file = await StorageFile.GetFileFromPathAsync(GetRoamingDirPath() + "\\appConfig.json");
                System.Diagnostics.Debug.WriteLine("FileManager: Config file OK");
            }
            catch(Exception)
            {
                System.Diagnostics.Debug.WriteLine("FileManager: Config file not exists");
                return false;
            }

            return true;
        }

        /// <summary>Function <c>ForceSaveConfigFile</c> gets ConfigDataModel class in parameter and
        /// force save it by overwriting existing file.</summary>
        ///
        public async Task ForceSaveConfigFile(ConfigDataModel filedata)
        {
            StorageFolder folder = await GetAppMainDir();
            var file = await folder.CreateFileAsync("appConfig.json", CreationCollisionOption.ReplaceExisting);
            var stream = await file.OpenAsync(FileAccessMode.ReadWrite);
            byte[] data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(filedata));

            await stream.WriteAsync(data.AsBuffer());
            stream.Dispose();
        }

        /// <summary>Function <c>ReadConfigFile</c> returns serialized config class, 
        /// which was read from file.</summary>
        ///
        public async Task<ConfigDataModel> ReadConfigFile()
        {
            try {
                StorageFolder folder = await GetAppMainDir();
                var file = await folder.OpenStreamForReadAsync("appConfig.json");

                byte[] dataIn = new byte[file.Length];
                await file.ReadAsync(dataIn, 0, (int)file.Length);

                System.Diagnostics.Debug.WriteLine(Encoding.UTF8.GetString(dataIn));

                ConfigDataModel configDataModel = new ConfigDataModel();
                try
                {
                    configDataModel = JsonConvert.DeserializeObject<ConfigDataModel>(Encoding.UTF8.GetString(dataIn));
                }catch(Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e);
                }

                return configDataModel;
            }
            catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }

            return null;
        }

        #endregion

        #region IMAGE SAVE FUNCTIONALITY

        //

        #endregion

    }
}
