﻿using Hiyakasudere.Data.Internal.MultiplatformInterfaces;
using System.Text;
using Windows.Storage;
using Hiyakasudere.Data.Internal.Config;
using Newtonsoft.Json;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Diagnostics;

namespace Hiyakasudere.Platforms.Windows.Functionality
{
    internal class FileManager : IFileManager
    {
        #region PATHS DEFINE
        /// <summary>Function <c>GetRoamingDirPath</c> returns app roaming path as string for hidden from user data</summary>
        ///
        protected string GetRoamingDirPath()
        {
            return ApplicationData.Current.RoamingFolder.Path;
        }

        /// <summary>Function <c>GetAppMainDir</c> returns app roaming path as StorageFolder for further use/summary>
        ///
        protected async Task<StorageFolder> GetAppMainDir()
        {
            System.Diagnostics.Debug.WriteLine(GetRoamingDirPath());
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

        #region MISC
        /// <summary>Function <c>OpenImagesDir</c> opens path .../Pictures/Hiyakasudere/ in explorer.exe </summary>
        ///
        public async Task OpenImagesDir()
        {
            var dir = await GetAppImagesDir();
            Process.Start("explorer.exe", dir.Path);
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
        /// <summary>Function <c>SaveImage</c> saves image in .../Pictures/Hiyakasudere/ directory 
        /// <para>base64Image - string representing image</para>
        /// <para>tags - image tags, NOT USED</para></summary>
        ///
        public async Task<bool> SaveImage(string base64Image, string tags)
        {
            StorageFolder folder = await GetAppImagesDir();
            var file = await folder.CreateFileAsync(tags + "." + "png", CreationCollisionOption.ReplaceExisting);
            var stream = await file.OpenAsync(FileAccessMode.ReadWrite);
            byte[] data = Convert.FromBase64String(base64Image);

            
            await stream.WriteAsync(data.AsBuffer());
            stream.Dispose();


            return true;
        }

        #endregion

    }
}
