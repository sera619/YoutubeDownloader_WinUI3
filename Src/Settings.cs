using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeDownloader.Src
{
    public class Settings
    {
        public string VersionNumber { get; set; }
        public string DefaultBasepath { get; set; }
        public string DefaultPlaylistName { get; set; } = "Playlists";
        public string DefaultPlaylistPath { get; set; }

        public string UserBasepath { get; set; }
        public string UserPlaylistName { get; set; }
        public string UserPlaylistPath { get; set; }
    }
}
