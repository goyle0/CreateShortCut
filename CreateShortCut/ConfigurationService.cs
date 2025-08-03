using System;
using System.Configuration;
using System.IO;

namespace CreateShortCut
{
    public interface IConfigurationService
    {
        string GetFolderPath();
        string GetDefaultPath();
        void SetFolderPath(string path);
        void SetDefaultPath(string path);
        bool HasFileWriteAccess(string filePath);
        void SaveConfiguration();
    }

    public class ConfigurationService : IConfigurationService
    {
        private Configuration _config;

        public ConfigurationService()
        {
            _config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        }

        public string GetFolderPath()
        {
            return ConfigurationManager.AppSettings["FolderPath"];
        }

        public string GetDefaultPath()
        {
            return ConfigurationManager.AppSettings["DefaultPath"];
        }

        public void SetFolderPath(string path)
        {
            _config.AppSettings.Settings["FolderPath"].Value = path;
        }

        public void SetDefaultPath(string path)
        {
            _config.AppSettings.Settings["DefaultPath"].Value = path;
        }

        public bool HasFileWriteAccess(string filePath)
        {
            try
            {
                // ファイルの存在確認
                if (!File.Exists(filePath))
                {
                    return false;
                }
                
                // 書き込み権限のテスト
                using (FileStream fs = File.Open(filePath, FileMode.Open, FileAccess.Write))
                {
                    return true;
                }
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
            catch (IOException)
            {
                return false;
            }
            catch
            {
                return false;
            }
        }

        public void SaveConfiguration()
        {
            _config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        public string GetConfigFilePath()
        {
            return _config.FilePath;
        }
    }
}