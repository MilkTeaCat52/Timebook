using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Timebook.Helper
{
    public static class DPIHelper
    {
        [DllImport("User32.dll")]
        public static extern uint GetDpiForWindow(IntPtr hWnd);

        public static double GetScaleForWindow(Window window)
        {
            var handle = WinRT.Interop.WindowNative.GetWindowHandle(window);
            uint dpi = GetDpiForWindow(handle);

            return (double)dpi / 96;
        }

    }
}
