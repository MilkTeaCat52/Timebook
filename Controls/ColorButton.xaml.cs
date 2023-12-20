using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Timebook.Controls
{
    public sealed partial class ColorButton : UserControl
    {
        public bool IsEmpty = true;
        bool canceled = false;

        new public Brush Background
        {
            get
            {
                return this.Button.Background;
            }

            set
            {
                this.Button.Background = value;
                ChangeHoverColor(((SolidColorBrush)value).Color);

                this.Picker.Color = ((SolidColorBrush)value).Color;

                if (IsEmpty)
                {
                    this.Picker.Color = Colors.White;
                }
            }
        }

        public ColorButton()
        {
            this.InitializeComponent();
        }

        public void ChangeHoverColor(Color originalColor)
        {
            bool bright = (originalColor.R > 128 || originalColor.G > 128 || originalColor.B > 128);

            byte A, R, G, B;

            if (!bright)
            {
                A = (byte)255;
                R = (byte)(originalColor.R + (255) * 0.05);
                G = (byte)(originalColor.G + (255) * 0.05);
                B = (byte)(originalColor.B + (255) * 0.05);
            }
            else
            {
                A = (byte)180;
                R = originalColor.R;
                G = originalColor.G;
                B = originalColor.B;
            }

            Color modifiedColor = Color.FromArgb(A, R, G, B);

            SolidColorBrush newBrush = new SolidColorBrush(modifiedColor);

            Button.Resources["ButtonBackgroundPointerOver"] = newBrush;
        }

        public void PickerOpened(object sender, object e)
        {
            canceled = false;
        }

        public void ColorPicked(object sender, object e)
        {
            if (!canceled)
            {
                IsEmpty = false;
                this.Background = new SolidColorBrush(this.Picker.Color);
            }
        }

        public void PickCanceled(object sender, object e)
        {
            canceled = true;
            this.Flyout.Hide();
        }
    }
}
