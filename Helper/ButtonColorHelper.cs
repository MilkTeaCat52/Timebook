using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                    A = (byte)(originalColor.A + 255 * 0.1);
                    R = originalColor.R;
                    G = originalColor.G;
                    B = originalColor.B;
                }
                else
                {
                    A = (byte)255;
                    R = (byte)(originalColor.R + 255 * 0.1);
                    G = (byte)(originalColor.G + 255 * 0.1);
                    B = (byte)(originalColor.B + 255 * 0.1);
                }
                //overflows
            }
            else //bright
            {
                A = (byte)(originalColor.A - 255 * 0.2);
                R = originalColor.R;
                G = originalColor.G;
                B = originalColor.B;
            }

            return new SolidColorBrush(Color.FromArgb(A, R, G, B));
        }

        public static Brush GetPressedBrush(Color originalColor)
        {
            byte A, R, G, B;


            A = (byte)(originalColor.A * 100 / 255);
            R = originalColor.R;
            G = originalColor.G;
            B = originalColor.B;

            return new SolidColorBrush(Color.FromArgb(A, R, G, B));
        }
    }
}
