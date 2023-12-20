using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using System;
using System.IO;
using Timebook.Helper;
using Windows.Storage;
using Windows.Storage.Pickers;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Timebook
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        string version = "Version " + App.Version.ToString();

        public SettingsPage()
        {
            this.InitializeComponent();
            Loaded += PageLoaded;

            this.StorageDirCard.SizeChanged += StorageDirCardSizeChanged;
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            var theme = SettingHelper.ThemeGet();
            switch (theme)
            {
                case ElementTheme.Light:
                    ThemeCombo.SelectedIndex = 0;
                    break;
                case ElementTheme.Dark:
                    ThemeCombo.SelectedIndex = 1;
                    break;
                case ElementTheme.Default:
                    ThemeCombo.SelectedIndex = 2;
                    break;
            }

            StorageDirBox.Text = SettingHelper.StoragePathGet();
        }

        private void ThemeToggled(object sender, RoutedEventArgs e)
        {
            switch (((ComboBoxItem)ThemeCombo.SelectedItem)?.Tag?.ToString())
            {
                case "Light":
                    ThemeHelper.RootTheme = ElementTheme.Light;
                    break;

                case "Dark":
                    ThemeHelper.RootTheme = ElementTheme.Dark;
                    break;

                case "System Default":
                    ThemeHelper.RootTheme = ElementTheme.Default;
                    break;
            }

            SettingHelper.ThemeSet(ThemeHelper.RootTheme);
        }

        private void StorageDirCardSizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.StorageDirBox.Width = Math.Max(this.StorageDirCard.ActualWidth - 203, 0);
        }
        private void StorageDirReset(object sender, RoutedEventArgs e)
        {
            SettingHelper.SetDefault("StoragePathSetting");
            StorageDirBox.Text = SettingHelper.StoragePathGet();
        }
        async private void StorageDirPick(object sender, RoutedEventArgs e)
        {
            var folderPicker = new FolderPicker();
            folderPicker.FileTypeFilter.Add("*");

            // Get the current window's HWND by passing in the Window object
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(WindowHelper.GetWindowForElement(this));

            // Associate the HWND with the file picker
            WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, hwnd);

            StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                StorageDirBox.Text = folder.Path;
            }
        }
        async private void StorageDirSave(object sender, RoutedEventArgs e)
        {
            var folderPath = StorageDirBox.Text;

            if (!Directory.Exists(folderPath))
            {
                var dialog = new ContentDialog();

                // XamlRoot must be set in the case of a ContentDialog running in a Desktop app
                dialog.XamlRoot = this.XamlRoot;
                dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
                dialog.PrimaryButtonText = "Yes";
                dialog.CloseButtonText = "Cancel";
                dialog.DefaultButton = ContentDialogButton.Close;
                dialog.Content = new TextBlock();

                dialog.Title = "The specified directory does not exist";

                ((TextBlock)dialog.Content).Text = "Should Timebook create a new directory?";
                ((TextBlock)dialog.Content).TextWrapping = TextWrapping.Wrap;

                var result = await dialog.ShowAsync();

                if (result == ContentDialogResult.Primary)
                {
                    try
                    {
                        Directory.CreateDirectory(folderPath);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        var dialog2 = new ContentDialog();

                        // XamlRoot must be set in the case of a ContentDialog running in a Desktop app
                        dialog2.XamlRoot = this.XamlRoot;
                        dialog2.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
                        dialog2.CloseButtonText = "OK";
                        dialog2.DefaultButton = ContentDialogButton.Close;
                        dialog2.Content = new TextBlock();
                        dialog2.Title = "Timebook was unable to create the specified directory";

                        ((TextBlock)dialog2.Content).Text = "The selected directory requires elevated permissions to access.\nPlease choose another directory.";
                        ((TextBlock)dialog2.Content).TextWrapping = TextWrapping.Wrap;

                        var result2 = await dialog2.ShowAsync();

                        return;
                    }
                    catch (Exception)
                    {
                        var dialog2 = new ContentDialog();

                        // XamlRoot must be set in the case of a ContentDialog running in a Desktop app
                        dialog2.XamlRoot = this.XamlRoot;
                        dialog2.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
                        dialog2.CloseButtonText = "OK";
                        dialog2.DefaultButton = ContentDialogButton.Close;
                        dialog2.Content = new TextBlock();
                        dialog2.Title = "Timebook was unable to create the specified directory";

                        ((TextBlock)dialog2.Content).Text = "An unexpected error occured.";
                        ((TextBlock)dialog2.Content).TextWrapping = TextWrapping.Wrap;

                        var result2 = await dialog2.ShowAsync();

                        return;
                    }
                }
                else
                {
                    return;
                }
            }

            //check if directory is writable
            try
            {
                var fpath = Path.Combine(folderPath, "tb.test.temp");
                if (File.Exists(fpath)) { File.Delete(fpath); }
                using (File.CreateText(fpath)) { }
                File.Delete(fpath);
            }
            catch (UnauthorizedAccessException)
            {
                var dialog = new ContentDialog();

                // XamlRoot must be set in the case of a ContentDialog running in a Desktop app
                dialog.XamlRoot = this.XamlRoot;
                dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
                dialog.CloseButtonText = "OK";
                dialog.DefaultButton = ContentDialogButton.Close;
                dialog.Content = new TextBlock();
                dialog.Title = "The selected directory requires elevated permissions to access";

                ((TextBlock)dialog.Content).Text = "Please choose another directory.";
                ((TextBlock)dialog.Content).TextWrapping = TextWrapping.Wrap;

                var result = await dialog.ShowAsync();

                return;
            }

            //move files
            try
            {
                var originalPath = SettingHelper.StoragePathGet();

                foreach (string file in Directory.GetFiles(originalPath))
                {
                    string fileName = Path.GetFileName(file);
                    string dest = Path.Combine(folderPath, fileName);
                    File.Move(file, dest);
                }
            }
            catch
            {
                var dialog = new ContentDialog();

                // XamlRoot must be set in the case of a ContentDialog running in a Desktop app
                dialog.XamlRoot = this.XamlRoot;
                dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
                dialog.CloseButtonText = "OK";
                dialog.DefaultButton = ContentDialogButton.Close;
                dialog.Content = new TextBlock();

                dialog.Title = "An error occured while trying to move files";

                ((TextBlock)dialog.Content).Text = "Please close all apps accessing files in the original directory and try again.";
                ((TextBlock)dialog.Content).TextWrapping = TextWrapping.Wrap;

                var result = await dialog.ShowAsync();

                return;
            }

            SettingHelper.StoragePathSet(folderPath);
        }
    }
}
