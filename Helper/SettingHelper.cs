using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Timebook.Helper
{
    public static class SettingHelper
    {
        static Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        static Dictionary<string,object> defaults= new Dictionary<string, object>()
        {
            {"ThemeSetting", (int)ElementTheme.Default},
            {"StoragePathSetting", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)+"\\Timebook" }
        };

        public static void SetDefault(string key)
        {
            localSettings.Values[key] = defaults[key];
        }

        public static void ThemeSet(ElementTheme rootTheme)
        {
            localSettings.Values["ThemeSetting"] = (int)rootTheme;
        }
        public static ElementTheme ThemeGet()
        {     
            if (localSettings.Values["ThemeSetting"] == null)
            {
                SetDefault("ThemeSetting");
            }
            return (ElementTheme)(localSettings.Values["ThemeSetting"]);
        }

        public static void StoragePathSet(string path)
        {
            localSettings.Values["StoragePathSetting"] = path;
        }
        public static string StoragePathGet()
        {
            if (localSettings.Values["StoragePathSetting"] == null)
            {
                SetDefault("StoragePathSetting");
            }
            return (string)localSettings.Values["StoragePathSetting"];
        }
    }
}
