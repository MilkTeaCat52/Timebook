using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using Timebook.Helper;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Timebook
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Settings : Window
    {
        double scale;

        int width = 800;
        int height = 500;

        [DllImport("User32.dll")]
        public static extern uint GetDpiForWindow(IntPtr hWnd);
        private double GetScale()
        {
            var handle = WinRT.Interop.WindowNative.GetWindowHandle(this);
            uint dpi = GetDpiForWindow(handle);

            return (double)dpi / 96;
        }

        public Settings(int x_mid, int y_mid)
        {
            WindowHelper.TrackWindow(this);

            scale = GetScale();

            var w = width * scale;
            var h = height * scale;
            AppWindow.MoveAndResize(new(x_mid - (int)(w / 2), y_mid - (int)(h / 2), (int)w, (int)h));
            this.InitializeComponent();

            ThemeHelper.ApplyTheme(this);

            ExtendsContentIntoTitleBar = true;   // enable custom titlebar
            SetTitleBar(AppTitleBar);           // set user ui element as titlebar
        }

        private void NavLoaded(object sender, RoutedEventArgs e)
        {
            // Add handler for ContentFrame navigation.
            ContentFrame.Navigated += ContentFrameNavigated;

            // Load default Page
            NavView.SelectedItem = NavView.MenuItems[0];
            NavNavigate(typeof(TimetablePage), new EntranceNavigationTransitionInfo());
        }
        private void NavSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected == true)
            {
                NavNavigate(typeof(SettingsPage), args.RecommendedNavigationTransitionInfo);
            }
            else if (args.SelectedItemContainer != null)
            {
                Type navPageType = Type.GetType(args.SelectedItemContainer.Tag.ToString());
                NavNavigate(navPageType, args.RecommendedNavigationTransitionInfo);
            }
        }
        private void NavNavigate(Type navPageType, NavigationTransitionInfo transitionInfo)
        {
            // Get the page type before navigation so you can prevent duplicate
            // entries in the backstack.
            Type preNavPageType = ContentFrame.CurrentSourcePageType;

            // Only navigate if the selected page isn't currently loaded.
            if (navPageType is not null && !Type.Equals(preNavPageType, navPageType))
            {
                ContentFrame.Navigate(navPageType, null, transitionInfo);
            }
        }
        private void ContentFrameNavigated(object sender, NavigationEventArgs e)
        {
            if (ContentFrame.SourcePageType == typeof(SettingsPage))
            {
                // SettingsItem is not part of NavView.MenuItems, and doesn't have a Tag.
                NavView.SelectedItem = (NavigationViewItem)NavView.SettingsItem;
            }
            else if (ContentFrame.SourcePageType != null)
            {
                // Select the nav view item that corresponds to the page being navigated to.
                NavView.SelectedItem = NavView.MenuItems
                            .OfType<NavigationViewItem>()
                            .First(i => i.Tag.Equals(ContentFrame.SourcePageType.FullName.ToString())); ;
            }
        }
    }
}
