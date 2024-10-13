using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace Timebook.Helper
{
    static class ColorHelper
    {
        public static long BrushToHex(Brush brush)
        {
            Color c = ((SolidColorBrush)brush).Color;

            //0xAARRGGBB
            long hex =
            ((long)c.A) * 0x1000000 +
            ((long)c.R) * 0x10000 +
            ((long)c.G) * 0x100 +
            ((long)c.B) * 0x1;

            return hex;
        }
        public static SolidColorBrush HexToBrush(long hex)
        {

            int b = (int)(hex % 0x100);
            int g = (int)(((hex - b) / 0x100) % 0x100);
            int r = (int)(((hex - b - g) / 0x10000) % 0x100);
            int a = (int)(((hex - b - g - r) / 0x1000000) % 0x100);

            SolidColorBrush brush = new SolidColorBrush(Color.FromArgb((byte)a, (byte)r, (byte)g, (byte)b));

            return brush;
        }

        public static byte GetBrightness(byte R, byte G, byte B)
        {
            return ((byte)(0.2126 * R + 0.7152 * G + 0.0722 * B));
        }

        public static Brush GetButtonHoverBrush(Color originalColor)
        {
            double alphaPercentage = (double)originalColor.A / 255;

            double backgroundHex = ThemeHelper.IsDarkTheme() ? 0x20 : 0xf9;

            byte mixedR = (byte)(originalColor.R * alphaPercentage + backgroundHex * (1 - alphaPercentage));
            byte mixedG = (byte)(originalColor.G * alphaPercentage + backgroundHex * (1 - alphaPercentage));
            byte mixedB = (byte)(originalColor.B * alphaPercentage + backgroundHex * (1 - alphaPercentage));


            bool mixedBright = (mixedR > 128 || mixedG > 128 || mixedB > 128);

            byte A, R, G, B;

            if (mixedBright) //bright
            {
                A = 255;
                R = (byte)(mixedR * 0.8);
                G = (byte)(mixedG * 0.8);
                B = (byte)(mixedB * 0.8);
            }
            else //dark
            {
                if (originalColor.A < 128) //handle transparency differently since the default button color is 0x0FFFFFFF
                {
                    A = (byte)((originalColor.A + 10) * 1.3);
                    R = originalColor.R;
                    G = originalColor.G;
                    B = originalColor.B;
                }
                else
                {
                    A = (byte)255;
                    R = (byte)(255 - (255 - originalColor.R) * 0.88);
                    G = (byte)(255 - (255 - originalColor.G) * 0.88);
                    B = (byte)(255 - (255 - originalColor.B) * 0.88);
                }
            }

            return new SolidColorBrush(Color.FromArgb(A, R, G, B));
        }

        public static Brush GetButtonPressedBrush(Color originalColor)
        {
            byte A, R, G, B;


            A = (byte)(originalColor.A);
            R = (byte)(originalColor.R * 0.4);
            G = (byte)(originalColor.G * 0.4);
            B = (byte)(originalColor.B * 0.4);

            return new SolidColorBrush(Color.FromArgb(A, R, G, B));
        }
    }
}
