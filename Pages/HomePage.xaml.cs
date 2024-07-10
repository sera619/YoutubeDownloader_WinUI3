using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace YoutubeDownloader.Pages
{
    public sealed partial class HomePage : Page
    {
        private readonly string InfoText = "YTDownloader is a powerful, user-friendly application designed to convert and download audio content from YouTube videos and playlists. Our software offers a seamless solution for music enthusiasts, podcast listeners, and content creators who wish to access their favorite audio offline.";
        public HomePage()
        {
            this.InitializeComponent();
            HomeInfoText.TextWrapping = TextWrapping.Wrap;
            HomeInfoText.Text = InfoText;
            string versionText = App.Configuration["VersionNumber"];
            HomeVersionText.Text = $"Version {versionText} © 2023 Development & Design by S3R43o3";
        }
    }
}
