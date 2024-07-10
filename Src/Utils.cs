using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace YoutubeDownloader.Src
{
    public static class Utils
    {
        public static bool IsValidYouTubeUrl(string url)
        {
            // YouTube URL pattern
            string pattern = @"^(https?\:\/\/)?(www\.youtube\.com|youtu\.?be)\/.+$";
            Regex regex = new (pattern, RegexOptions.IgnoreCase);
            return regex.IsMatch(url);
        }

    }
}
