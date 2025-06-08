using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml;
using Microsoft.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.ViewManagement;

namespace Timebook.Helper
{
    public static class RefreshHelper {
    
        public delegate void UIRefreshedHandler(object sender, EventArgs e);
        public static event UIRefreshedHandler UIRefreshed;

        static RefreshHelper()
        {

        }

        public static void RefreshUI()
        {
            UIRefreshed?.Invoke(null, null);
        }
    }
}
