using Microsoft.UI.Xaml.Media;
using System;
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

        private class FiveColor
        {
            public byte A;
            public double L;
            public byte R;
            public byte G;
            public byte B;

            public FiveColor(byte A, byte R, byte G, byte B)
            {
                this.A = A;

                double Max = Math.Max(Math.Max(R, G), B);
                this.L = Max / 255;
                this.R = (byte)(R / this.L);
                this.B = (byte)(B / this.L);
                this.G = (byte)(G / this.L);

                if (this.R == 0 && this.G == 0 && this.B == 0)
                {
                    this.R = this.G = this.B = 255;
                }
            }

            public FiveColor(Color color)
            {
                this.A = color.A;

                var R = color.R;
                var G = color.G;
                var B = color.B;

                double Max = Math.Max(Math.Max(R, G), B);
                this.L = Max / 255;
                this.R = (byte)(R / this.L);
                this.B = (byte)(B / this.L);
                this.G = (byte)(G / this.L);

                if (this.R == 0 && this.G == 0 && this.B == 0)
                {
                    this.R = this.G = this.B = 255;
                }

            }

            public static FiveColor FromBlend(Color frontColor, Color backColor)
            {
                double alphaPercentage = (double)frontColor.A / 255;
                var R = (byte)(frontColor.R * alphaPercentage + backColor.R * (1 - alphaPercentage));
                var G = (byte)(frontColor.G * alphaPercentage + backColor.G * (1 - alphaPercentage));
                var B = (byte)(frontColor.B * alphaPercentage + backColor.B * (1 - alphaPercentage));

                return new FiveColor(255, R, G, B);
            }

            public bool IsBright()
            {
                return L >= 0.5;
            }

            public void ShiftAlphaPositive(byte value)
            {
                A += value;
            }

            public void ShiftAlphaNegative(byte value)
            {
                A -= value;
            }

            public void ShiftBrightness(double value)
            {
                L += value / 100;
            }

            public void ScaleBrightness(double scale)
            {
                L *= scale;
            }

            public void FlipBrightness()
            {
                L = 1 - L;
            }

            public Color ToColor()
            {
                return Color.FromArgb(A, (byte)(R * L), (byte)(G * L), (byte)(B * L));
            }
        }

        public static Brush GetButtonHoverBrush(Color originalColor)
        {
            var fiveColor = new FiveColor(originalColor);

            long backgroundHex = ThemeHelper.IsDarkTheme() ? 0xFF202020 : 0xFFf9f9f9;
            var mixedFiveColor = FiveColor.FromBlend(originalColor,ColorHelper.HexToBrush(backgroundHex).Color);

            if (mixedFiveColor.IsBright()) //bright
            {
                if (fiveColor.A < 128)
                {
                    if (fiveColor.IsBright())
                    {
                        fiveColor.FlipBrightness();
                    }
                    fiveColor.ShiftAlphaPositive(18);
                }
                else
                {
                    fiveColor.ShiftBrightness(-20);
                }
            }
            else //dark
            {
                if (fiveColor.A < 128) //handle transparency differently since the default button color is 0x0FFFFFFF
                {
                    if (!fiveColor.IsBright())
                    {
                        fiveColor.FlipBrightness();
                    }
                    fiveColor.ShiftAlphaPositive(18);
                }
                else
                {
                    if (ThemeHelper.IsDarkTheme())
                    {
                        double threshold = 10;

                        if (fiveColor.L < threshold / 100)
                        {
                            fiveColor.L = 0.13;
                        }
                        else
                        {
                            fiveColor.ShiftBrightness(10);
                        }
                    }
                    else
                    {
                        double threshold = 10;

                        if (fiveColor.L < threshold / 100)
                        {
                            fiveColor.ShiftAlphaNegative(50);
                        }
                        else
                        {
                            fiveColor.ShiftAlphaNegative(40);
                        }
                    }
                }
            }


            return new SolidColorBrush(fiveColor.ToColor());
        }

        public static Brush GetButtonPressedBrush(Color originalColor)
        {
            var fiveColor = new FiveColor(originalColor);

            fiveColor.ScaleBrightness(0.4);

            return new SolidColorBrush(fiveColor.ToColor());
        }
    }
}
