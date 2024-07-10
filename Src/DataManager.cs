using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace YoutubeDownloader.Src
{
    public static class DataManager
    {

        private static readonly string _versionNumber = "3.8.4";
        public static string BaseFolderName { set; get; } = "YTDownloader";
        public static string PlaylistFolderName { set; get; } = "Playlists";
        public static string SettingsFileName { set; get; } = "appsettings.json";
        public static string BaseFolderPath { set; get; }
        public static string PlaylistFolderPath { set; get; }
        public static string SettingsFilePath { set;  get; }



        private static void GenerateDirectorys()
        {
            BaseFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic), BaseFolderName);
            PlaylistFolderPath = Path.Combine(BaseFolderPath, PlaylistFolderName);
            SettingsFilePath = Path.Combine(BaseFolderPath, SettingsFileName);

            if (!Directory.Exists(BaseFolderPath))
            {
                Directory.CreateDirectory(BaseFolderPath);
            }

            if (!Directory.Exists(PlaylistFolderPath))
            {
                Directory.CreateDirectory(PlaylistFolderPath);
            }
        }

        public static void SaveSetting(string key, string value)
        {
            var json = File.ReadAllText(SettingsFilePath);
            dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
            jsonObj[key] = value;
            string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(SettingsFilePath, output);
        }


        public static void CreateDefaultSettings()
        {
            GenerateDirectorys();

            if (!File.Exists(SettingsFilePath))
            {

                var defaultSetting = new Settings
                {
                    VersionNumber = _versionNumber,

                    DefaultBasepath = BaseFolderPath,
                    DefaultPlaylistName = PlaylistFolderName,
                    DefaultPlaylistPath = PlaylistFolderPath,

                    UserBasepath = BaseFolderPath,
                    UserPlaylistName = PlaylistFolderName,
                    UserPlaylistPath = PlaylistFolderPath,
                };

                string jsonString = JsonSerializer.Serialize(defaultSetting, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(SettingsFilePath, jsonString);
            }
        }

    }

}

