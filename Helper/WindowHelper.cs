using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using WinRT.Interop;

namespace Timebook.Helper
{
    public static class WindowHelper
    {
        static public void InitializeWindow(Window window, int width, int height, UIElement titleBar)
        {
            TrackWindow(window);

            var scale = DPIHelper.GetScaleForWindow(window);
            window.AppWindow.Resize(new((int)(width * scale), (int)(height * scale)));

            ThemeHelper.SubscribeToThemeChange(window);

            window.ExtendsContentIntoTitleBar = true;   // enable custom titlebar
            window.SetTitleBar(titleBar);               // set user ui element as titlebar
        }

        static public void TrackWindow(Window window)
        {
            window.Closed += (sender, args) => {
                _activeWindows.Remove(window);
            };
            _activeWindows.Add(window);
        }

        static public Window GetWindowForElement(UIElement element)
        {
            if (element.XamlRoot != null)
            {
                foreach (Window window in _activeWindows)
                {
                    if (element.XamlRoot == window.Content.XamlRoot)
                    {
                        return window;
                    }
                }
            }
            return null;
        }

        static public List<Window> ActiveWindows { get { return _activeWindows; } }

        static private List<Window> _activeWindows = new List<Window>();
    }
}
