using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace Timebook.Helper
{
    static class ButtonColorHelper
    {
        public static Brush GetHoverBrush(Color originalColor)
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

        public static Brush GetPressedBrush(Color originalColor)
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
