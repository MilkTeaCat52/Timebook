using Microsoft.UI;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using Windows.UI;
using Windows.UI.ViewManagement;


namespace Timebook.Helper
{
    public static class ThemeHelper
    {
        static DispatcherQueue dispatcherQueue;
        static UISettings UISettings;

        static ElementTheme _rootTheme;

        public delegate void ThemeChangedHandler(object sender, EventArgs e);
        public static event ThemeChangedHandler ThemeChanged;

        static ThemeHelper()
        {
            RootTheme = SettingHelper.ThemeGet();

            //Automate theme switching when theme change detected
            dispatcherQueue = DispatcherQueue.GetForCurrentThread();
            UISettings = new UISettings();
            UISettings.ColorValuesChanged += OnSystemThemeChanged;
        }

        public static void OnSystemThemeChanged(UISettings sender, object args)
        {
            dispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.High,
            () =>
            {
                ThemeChanged?.Invoke(null, null);
            });
        }

        public static ElementTheme RootTheme
        {
            get
            {
                return _rootTheme;
            }
            set
            {
                _rootTheme = value;
                ThemeChanged?.Invoke(null, null);
            }
        }

        public static void SubscribeToThemeChange(ContentControl control)
        {
            void ApplyThemeToControl(object sender=null, EventArgs e=null)
            {
                control.RequestedTheme = RootTheme;
            }

            ApplyThemeToControl();

            ThemeChanged += ApplyThemeToControl;

            control.Unloaded += (object sender, RoutedEventArgs e) =>
            {
                ThemeChanged -= ApplyThemeToControl;
            };

        }

        public static void SubscribeToThemeChange(Window window)
        {
            void ApplyThemeToWindow(object sender=null, EventArgs e=null)
            {
                if (window.Content is FrameworkElement rootElement)
                {
                    rootElement.RequestedTheme = RootTheme;
                }
                ApplyCaptionButtonColors(window);
            }

            ApplyThemeToWindow();

            ThemeChanged += ApplyThemeToWindow;

            window.Closed += (object sender, WindowEventArgs e) =>
            {
                ThemeChanged -= ApplyThemeToWindow;
            };

        }

        private static void ApplyCaptionButtonColors(Window window)
        {
            if (IsDarkTheme())
            {
                window.AppWindow.TitleBar.ButtonForegroundColor = Colors.White;
                window.AppWindow.TitleBar.ButtonHoverForegroundColor = Colors.White;
                window.AppWindow.TitleBar.ButtonPressedForegroundColor = Colors.White;

                //window.AppWindow.TitleBar.ButtonInactiveForegroundColor = Color.FromArgb(0xFF, 0x66, 0x66, 0x66);
                Application.Current.Resources["WindowCaptionForegroundDisabled"] = new SolidColorBrush(Color.FromArgb(0x66, 0x00, 0x00, 0x00));

                window.AppWindow.TitleBar.ButtonHoverBackgroundColor = Color.FromArgb(0x18, 0xFF, 0xFF, 0xFF);
                window.AppWindow.TitleBar.ButtonPressedBackgroundColor = Color.FromArgb(0x33, 0xFF, 0xFF, 0xFF);
            }
            else
            {
                window.AppWindow.TitleBar.ButtonForegroundColor = Colors.Black;
                window.AppWindow.TitleBar.ButtonHoverForegroundColor = Colors.Black;
                window.AppWindow.TitleBar.ButtonPressedForegroundColor = Colors.Black;

                //window.AppWindow.TitleBar.ButtonInactiveForegroundColor = Color.FromArgb(0xFF, 0x7A, 0x7A, 0x7A);
                Application.Current.Resources["WindowCaptionForegroundDisabled"] = new SolidColorBrush(Color.FromArgb(0x66, 0xFF, 0xFF, 0xFF));

                window.AppWindow.TitleBar.ButtonHoverBackgroundColor = Color.FromArgb(0x18, 0x00, 0x00, 0x00);
                window.AppWindow.TitleBar.ButtonPressedBackgroundColor = Color.FromArgb(0x33, 0x00, 0x00, 0x00);
            }
        }

        public static bool IsDarkTheme()
        {
            if (RootTheme == ElementTheme.Default)
            {
                return Application.Current.RequestedTheme == ApplicationTheme.Dark;
            }
            return RootTheme == ElementTheme.Dark;
        }
    }
}
