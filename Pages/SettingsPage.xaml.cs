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
using Windows.Storage.Pickers;
using Windows.Storage;
using Windows.System;
using WinRT.Interop;
using YoutubeDownloader;
using YoutubeDownloader.Src;


namespace YoutubeDownloader.Pages
{
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            this.InitializeComponent();
            string baseFolderPath = App.Configuration["UserBasepath"];
            string playlistSavepath = App.Configuration["UserPlaylistPath"];
            string versionNumber = App.Configuration["VersionNumber"];
            VersionTextBlock.Text = $"Version {versionNumber}";
            PlaylistSavepathBox.Text = playlistSavepath;
            SingleSavepathBox.Text = baseFolderPath;
        }

        private void SaveSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            string newPlaylistSavePath = PlaylistSavepathBox.Text;
            string newSingleSavePath = SingleSavepathBox.Text;
            if (newPlaylistSavePath != string.Empty)
            {
                DataManager.SaveSetting("UserPlaylistPath", newPlaylistSavePath);
                if (!Directory.Exists(newPlaylistSavePath)) 
                { 
                    Directory.CreateDirectory(newPlaylistSavePath);
                }
            }
            if (newSingleSavePath != string.Empty)
            {
                DataManager.SaveSetting("UserBasepath", newSingleSavePath);
                if (!Directory.Exists(newSingleSavePath))
                {
                    Directory.CreateDirectory(newSingleSavePath);
                }
            }
        }

        private void ResetDefaultButton_Click(object sender, RoutedEventArgs e)
        {
            string defaultBasePath = App.Configuration["DefaultBasepath"];
            string defaultPlaylistName = App.Configuration["DefaultPlaylistName"];
            string defaultPlaylistPath = App.Configuration["DefaultPlaylistPath"];

            DataManager.SaveSetting("UserBasepath", defaultBasePath);
            DataManager.SaveSetting("UserPlaylistPath", defaultPlaylistPath);
            DataManager.SaveSetting("UserPlaylistName", defaultPlaylistName);

            PlaylistSavepathBox.Text = defaultPlaylistPath;
            SingleSavepathBox.Text = defaultBasePath;
        }

        private async void ChangeSingleSavepathButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FolderPicker folderPicker = new ();
                folderPicker.SuggestedStartLocation = PickerLocationId.MusicLibrary;
                folderPicker.FileTypeFilter.Add("*");
                var hwnd = WindowNative.GetWindowHandle(App.Main_Window); 
                InitializeWithWindow.Initialize(folderPicker, hwnd);
                StorageFolder folder = await folderPicker.PickSingleFolderAsync();
                if (folder != null)
                {
                    SingleSavepathBox.Text = folder.Path;
                }
            }
            catch (Exception ex)
            {
                ContentDialog errorDialog = new ()
                {
                    Title = "Error",
                    Content = ex.Message,
                    CloseButtonText = "OK"
                };
                errorDialog.XamlRoot = this.XamlRoot;
                await errorDialog.ShowAsync();
            }
        }

        private async void ChangePlaylistSavepathButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FolderPicker folderPicker = new ();
                folderPicker.SuggestedStartLocation = PickerLocationId.MusicLibrary;
                folderPicker.FileTypeFilter.Add("*");
                var hwnd = WindowNative.GetWindowHandle(App.Main_Window);
                InitializeWithWindow.Initialize(folderPicker, hwnd);
                StorageFolder folder = await folderPicker.PickSingleFolderAsync();
                if (folder != null)
                {
                    PlaylistSavepathBox.Text = folder.Path;
                }
            }
            catch (Exception ex)
            {
                ContentDialog errorDialog = new ()
                {
                    Title = "Error",
                    Content = ex.Message,
                    CloseButtonText = "OK"
                };
                errorDialog.XamlRoot = this.XamlRoot;
                await errorDialog.ShowAsync();
            }
        }
    }
}
