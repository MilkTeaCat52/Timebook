using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Timebook.Controls;
using System.Runtime.CompilerServices;
using Microsoft.UI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Timebook.Controls
{
    public sealed partial class Cell : UserControl
    {
        bool mutable = false;
        bool isempty = false;

        public bool empty
        {
            get
            {
                return isempty;
            }

            set
            {
                isempty = value;
            }
        }

        public bool EditMode
        {
            get
            {
                return mutable;
            }

            set
            {
                this.Button.IsHitTestVisible = value;
                this.Button.IsTabStop = value;
                mutable = value;
            }
        }

        public Cell()
        {
            this.InitializeComponent();
            this.EditMode = false;
            this.Icon.Foreground = null;
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

        public void MouseEntered(object sender, RoutedEventArgs e)
        {
            if (mutable && empty)
            {
                this.Icon.Foreground = new SolidColorBrush(Colors.White);
            }
        }

        public void MouseExited(object sender, RoutedEventArgs e)
        {
            this.Icon.Foreground = null;
        }

    }
}
