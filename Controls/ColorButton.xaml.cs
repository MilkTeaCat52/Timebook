using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Timebook.Helper;
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

                var color = ((SolidColorBrush)value).Color;

                Button.Resources["ButtonBackgroundPointerOver"] = Helper.ColorHelper.GetButtonHoverBrush(color);
                Button.Resources["ButtonBackgroundPressed"] = Helper.ColorHelper.GetButtonPressedBrush(color);

                this.Picker.Color = IsEmpty ? Colors.White : color;
            }
        }

        public ColorButton()
        {
            this.InitializeComponent();
        }

        public void PickerOpened(object sender, object e)
        {
            canceled = true;
        }

        public void ColorPicked(object sender, object e)
        {
            if (!canceled)
            {
                IsEmpty = false;
                this.Background = new SolidColorBrush(this.Picker.Color);
            }
        }

        public void PickSaved(object sender, object e)
        {
            canceled = false;
            this.Flyout.Hide();
        }
    }
}
