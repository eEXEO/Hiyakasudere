using Hiyakasudere.Data.Internal.Config;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Hiyakasudere.Data.Internal.MultiplatformInterfaces
{
    /// <summary>
    /// Cross-platform file manager that works on Windows, Linux, and macOS.
    /// </summary>
    public class CrossPlatformFileManager : IFileManager
    {
        private string GetAppDataDir()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var dir = Path.Combine(appData, "Hiyakasudere");
            Directory.CreateDirectory(dir);
            return dir;
        }

        private string GetImagesDir()
        {
            var pictures = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            var dir = Path.Combine(pictures, "Hiyakasudere");
            Directory.CreateDirectory(dir);
            return dir;
        }

        private string GetConfigFilePath() => Path.Combine(GetAppDataDir(), "appConfig.json");

        public Task<bool> IsConfigFilePresent()
        {
            return Task.FromResult(File.Exists(GetConfigFilePath()));
        }

        public Task OpenImagesDir()
        {
            var dir = GetImagesDir();
            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    Process.Start("explorer.exe", dir);
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    Process.Start("xdg-open", dir);
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    Process.Start("open", dir);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Failed to open directory: " + ex.Message);
            }
            return Task.CompletedTask;
        }

        public async Task ForceSaveConfigFile(ConfigDataModel filedata)
        {
            var json = JsonConvert.SerializeObject(filedata, Formatting.Indented);
            await File.WriteAllTextAsync(GetConfigFilePath(), json);
        }

        public async Task<ConfigDataModel> ReadConfigFile()
        {
            try
            {
                var json = await File.ReadAllTextAsync(GetConfigFilePath());
                return JsonConvert.DeserializeObject<ConfigDataModel>(json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Failed to read config: " + ex.Message);
                return null;
            }
        }

        public async Task<bool> SaveImage(string base64Image, string filename)
        {
            try
            {
                var dir = GetImagesDir();
                var filePath = Path.Combine(dir, filename + ".png");
                var bytes = Convert.FromBase64String(base64Image);
                await File.WriteAllBytesAsync(filePath, bytes);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Failed to save image: " + ex.Message);
                return false;
            }
        }
    }
}
