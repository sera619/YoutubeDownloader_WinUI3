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

namespace YoutubeDownloader.Pages
{
    public sealed partial class DownloadSinglePage : Page
    {
        public ObservableCollection<VideoInfo> Videos { get; set; }

        public DownloadSinglePage()
        {
            this.InitializeComponent();
            Videos = [];
            VideoListView.ItemsSource = Videos;
            string versionText = App.Configuration["VersionNumber"];
            VersionTextBlock.Text = $"Version {versionText}";
        }
        
        private void RemoveEntry_Click(object sender, RoutedEventArgs e)
        {
            if(sender is Button button && button.Tag is VideoInfo videoInfo)
            {
                Videos.Remove(videoInfo);
            }
        }

        private async void AddVideoToList(string URL)
        {
            try
            {
                VideoProgessRing.IsActive = true;
                StatusTextBlock.Text = "Add new video to list...";
                YoutubeClient client = new ();
                var video = await client.Videos.GetAsync(URL);
                Videos.Add(new VideoInfo
                {
                    Url = URL,
                    Title = video.Title,
                    Duration = video.Duration?.ToString() ?? "Unknown"
                });
                VideoProgessRing.IsActive = false;
                StatusTextBlock.Text = string.Empty;
            }
            catch (Exception ex)
            {
                VideoProgessRing.IsActive = false;
                StatusTextBlock.Text = $"Error occurred: {ex.Message}";
            }
        }

        private async Task DownloadAllVideos()
        {
            if (Videos.Count == 0)
            {
                return;
            }
            int maxCount = Videos.Count;
            int successCount = 0;
            int failCount = 0;
            int currentCount = 0;
            VideoProgessRing.IsActive = true;
            foreach (var video in Videos.ToList())  // Use ToList() to avoid collection modification issues
            {
                try
                {
                    currentCount++;
                    StatusTextBlock.Text = $"Downloading video {currentCount}/{maxCount}...";
                    YoutubeClient client = new ();
                    var streamManifest = await client.Videos.Streams.GetManifestAsync(video.Url);
                    var audioStreamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();
                    var audioStream = await client.Videos.Streams.GetAsync(audioStreamInfo);
                    string audioFilePath = Path.Combine(DataManager.BaseFolderPath, $"{video.Title}.mp3");
                    await using (var outputFileStream = File.OpenWrite(audioFilePath))
                    {
                        await audioStream.CopyToAsync(outputFileStream);
                    }
                    successCount++;
                    StatusTextBlock.Text = $"Audio successfully downloaded!";
                    Videos.Remove(video);
                }
                catch (Exception ex)
                {
                    failCount++;
                    Videos.Remove(video);
                    StatusTextBlock.Text = $"Error occurred: {ex.Message}";
                }
            }
            StatusTextBlock.Text = string.Empty;
            VideoProgessRing.IsActive = false;
        }

        private void AddToSingleListButton_Click(object sender, RoutedEventArgs e)
        {
            string url = UrlBox.Text;
            if (url != null && Utils.IsValidYouTubeUrl(url))
            {
                UrlBox.Text = string.Empty;
                AddVideoToList(url);
            }
        }

        private async void DownloadSingleVideoButton_Click(object sender, RoutedEventArgs e)
        {
            if (Videos.Count > 0)
            {
                await DownloadAllVideos();
            }
        }

        private void ClearSingleListButton_Click(object sender, RoutedEventArgs e)
        {
            if (Videos.Count > 0)
            { 
                Videos.Clear();
            }
        }

        private void OpenSingleSavepathButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", App.Configuration["UserBasepath"]);
        }
    }
}
