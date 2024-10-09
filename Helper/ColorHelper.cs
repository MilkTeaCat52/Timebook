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
        public static Brush GetButtonHoverBrush(Color originalColor)
        {
            double alphaPercentage = (double)originalColor.A / 255;

            bool bright = (originalColor.R > 128 || originalColor.G > 128 || originalColor.B > 128);
            bool alphaBright = (originalColor.R * alphaPercentage > 128 || originalColor.G * alphaPercentage > 128 || originalColor.B * alphaPercentage > 128);

            byte A, R, G, B;

            if (!alphaBright) //not bright
            {
                if (bright) //prevent overflow from adding to color
                {
                    A = (byte)((originalColor.A + 10) * 1.3);
                    R = originalColor.R;
                    G = originalColor.G;
                    B = originalColor.B;
                }
                else
                {
                    A = (byte)255;
                    R = (byte)((originalColor.R + 10) * 1.3);
                    G = (byte)((originalColor.G + 10) * 1.3);
                    B = (byte)((originalColor.B + 10) * 1.3);
                }
                //overflows
            }
            else //bright
            {
                A = (byte)(originalColor.A);
                R = (byte)(originalColor.R * 0.7);
                G = (byte)(originalColor.G * 0.7);
                B = (byte)(originalColor.B * 0.7);
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
