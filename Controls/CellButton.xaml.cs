using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using Timebook.Helper;
using Windows.ApplicationModel.DataTransfer;

using ClassID = System.Guid;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Timebook.Controls
{
    public sealed partial class CellButton : UserControl
    {
        public bool isEmpty = true;

        ClassID classDataID; //migrate after structures implemented

        new public Brush Background
        {
            get
            {
                return this.Button.Background;
            }

            set
            {
                this.Button.Background = value;

                var color = ((SolidColorBrush)value).Color;

                Button.Resources["ButtonBackgroundPointerOver"] = Helper.ColorHelper.GetButtonHoverBrush(color);
                Button.Resources["ButtonBackgroundPressed"] = Helper.ColorHelper.GetButtonPressedBrush(color);
            }
        }
        public string Text
        {
            set
            {
                this.TextBlock.Text = value;
            }
        }

        public CellButton()
        {
            this.InitializeComponent();

            this.Icon.Foreground = null;
            LoadContent();
            this.ActualThemeChanged += LoadContent;
        }

        public void CellEdit(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new ContentDialog();

            // XamlRoot must be set in the case of a ContentDialog running in a Desktop app
            dialog.XamlRoot = this.XamlRoot;
            dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
            dialog.Title = "Edit Cell";
            dialog.PrimaryButtonText = "Save";
            dialog.CloseButtonText = "Cancel";
            dialog.DefaultButton = ContentDialogButton.Primary;
            dialog.Content = new CellEditPage();

            var result = dialog.ShowAsync();

        }

        public void LoadContent(FrameworkElement sender = null, object e = null)
        {
            if (isEmpty)
            {
                this.Background = this.Background; //sets background for "+" button
            }
            else
            {
                this.Icon.Foreground = null;

                var dataTemp = DataHelper.GetClassData(classDataID);

                this.Background = Timebook.Helper.ColorHelper.HexToBrush(dataTemp.Color);
                this.Text = dataTemp.Name;
            }
        }

        public void MouseEntered(object sender, RoutedEventArgs e)
        {
            if (isEmpty)
            {
                if (ThemeHelper.IsDarkTheme())
                {
                    this.Icon.Foreground = new SolidColorBrush(Colors.White);
                }
                else
                {
                    this.Icon.Foreground = new SolidColorBrush(Colors.Black);
                }
            }
        }

        public void MouseExited(object sender, RoutedEventArgs e)
        {
            this.Icon.Foreground = null;
        }

        private void OnDragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Copy;
        }

        private async void OnDrop(object sender, DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.Text))
            {
                string droppedItem = await e.DataView.GetTextAsync();
                ClassID id = ClassID.Parse(droppedItem);

                if (id != ClassID.Empty)
                {
                    this.isEmpty = false;
                    this.classDataID = id;
                    LoadContent();
                }
            }
        }

    }
}
