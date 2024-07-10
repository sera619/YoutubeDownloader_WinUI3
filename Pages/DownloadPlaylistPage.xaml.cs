using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using YoutubeDownloader.Src;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace YoutubeDownloader.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DownloadPlaylistPage : Page
    {
        public ObservableCollection<VideoInfo> PlaylistVideos { get; set; }
        public DownloadPlaylistPage()
        {
            this.InitializeComponent();
            PlaylistVideos = [];
            PlaylistVideoListView.ItemsSource = PlaylistVideos;
            string versionText = App.Configuration["VersionNumber"];
            VersionTextBlock.Text = $"Version {versionText}";                    
        }


        private async Task DownloadPlaylistVideos()
        {
            if (PlaylistVideos.Count == 0)
            {
                return;
            }
            string playlistName = DirectoryNameBox.Text;
            string playlistSavePath = Path.Combine(App.Configuration["UserPlaylistPath"], playlistName);

            Directory.CreateDirectory(playlistSavePath);

            int maxCount = PlaylistVideos.Count;
            int currentCount = 0;
            int successCount = 0;
            int failCount = 0;

            VideoProgessRing.IsActive = true;
            foreach (var video in PlaylistVideos.ToList())
            {
                try
                {
                    currentCount++;
                    YoutubeClient client = new ();
                    StatusTextBlock.Text = $"Downloading File {currentCount}/{maxCount}...";
                    var streamManifest = await client.Videos.Streams.GetManifestAsync(video.Url);
                    var audioStreamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();
                    var audioStream = await client.Videos.Streams.GetAsync(audioStreamInfo);

                    string audioFilePath = Path.Combine(playlistSavePath, $"{video.Title}.mp3");
                    await using (var outputFileStream = File.OpenWrite(audioFilePath))
                    {
                        await audioStream.CopyToAsync(outputFileStream);
                    }
                    successCount++;
                    PlaylistVideos.Remove(video);
                }
                catch (Exception ex)
                {
                    failCount++;
                    StatusTextBlock.Text = $"Error occurred: {ex.Message}";
                    PlaylistVideos.Remove(video);
                }
            }
            VideoProgessRing.IsActive = false;
            DirectoryNameBox.Text = string.Empty;
            StatusTextBlock.Text = $"Download complete. Success: {successCount}, Failed: {failCount}";
            OpenPlaylistPath(playlistSavePath);
        }

        private async void GetPlaylistInfos(string URL)
        {
            try
            {
                VideoProgessRing.IsActive = true;
                StatusTextBlock.Text = "Get Playlist Metadata...";
                YoutubeClient client = new ();

                // Get playlist info
                var playlist = await client.Playlists.GetAsync(URL);
                DirectoryNameBox.Text = playlist.Title;

                await foreach (var video in client.Playlists.GetVideosAsync(URL))
                {
                    PlaylistVideos.Add(new VideoInfo
                    {
                        Title = video.Title,
                        Url = video.Url,
                        Duration = video.Duration?.ToString()
                    });
                }

                VideoProgessRing.IsActive = false;
                StatusTextBlock.Text = string.Empty;
            }
            catch (Exception ex)
            {
                VideoProgessRing.IsActive = false;
                StatusTextBlock.Text = $"Error occured: {ex.Message}";
            }
        }

        private static void OpenPlaylistPath(string playlistPath)
        {
            Process.Start("explorer.exe", playlistPath);
        }

        private void GetPlaylistVideosButton_Click(object sender, RoutedEventArgs e)
        {
            var url = PlaylistUrlBox.Text;
            if(url != null)
            {
                PlaylistUrlBox.Text = string.Empty;
                GetPlaylistInfos(url);
            }
            else
            {
                return;
            }
        }

        private void ClearPlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            PlaylistVideos.Clear();
            PlaylistUrlBox.Text= string.Empty;
            DirectoryNameBox.Text = string.Empty;
        }

        private async void DownloadPlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            await DownloadPlaylistVideos();
        }

        private void OpenPlaylistSavepath_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", App.Configuration["UserPlaylistPath"]);
        }
    }
}
