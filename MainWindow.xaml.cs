using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

using System.Runtime.InteropServices;
using Timebook.Controls;
using Timebook.Helper;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Timebook
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        Settings settings = null;

        double scale;

        int width = 960;
        int height = 600;


        [DllImport("User32.dll")]
        public static extern uint GetDpiForWindow(IntPtr hWnd);
        private double GetScale()
        {
            var handle = WinRT.Interop.WindowNative.GetWindowHandle(this);
            uint dpi = GetDpiForWindow(handle);

            return (double)dpi / 96;
        }

        public MainWindow()
        {
            WindowHelper.TrackWindow(this);

            scale = GetScale();

            this.AppWindow.Resize(new((int)(width * this.scale), (int)(height * this.scale)));
            this.InitializeComponent();

            ThemeHelper.ApplyTheme(this);

            ExtendsContentIntoTitleBar = true;   // enable custom titlebar
            SetTitleBar(AppTitleBar);           // set user ui element as titlebar

            this.Closed += OnMainWindowClosed;
        }

        private void OpenSettings(object sender, RoutedEventArgs e)
        {
            if (settings == null)
            {
                var x_mid = this.AppWindow.Position.X + this.AppWindow.Size.Width / 2;
                var y_mid = this.AppWindow.Position.Y + this.AppWindow.Size.Height / 2;

                settings = new Settings(x_mid, y_mid);

                settings.Closed += OnSettingsClosed;

                settings.InitializeComponent();
            }
            settings.Activate();
        }

        private void OnSettingsClosed(object sender, WindowEventArgs args)
        {
            this.settings = null;
        }

        private void OnMainWindowClosed(object sender, WindowEventArgs e)
        {
            if (settings != null)
            {
                settings.Close();
            }
        }
    }
}
