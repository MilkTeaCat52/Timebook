using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using Timebook.Helper;
using Windows.UI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Timebook.Controls
{
    public class GroupData
    {
        public long Color { get; set; } = -0x1;
        public string Name { get; set; } = "";
        public string Teacher { get; set; } = "";
        public string Room { get; set; } = "";
    }

    public sealed partial class Group : UserControl
    {
        ContentDialog dialog;

        Guid id;
        GroupData data;

        public bool IsEmpty = true;

        new public Brush Background
        {
            set
            {
                this.Button.Background = value;
                ChangeHoverColor(((SolidColorBrush)value).Color);
            }

            get
            {
                return this.Button.Background;
            }
        }
        public string Text
        {
            set
            {
                this.TextBlock.Text = value;
            }

            get
            {
                return this.TextBlock.Text;
            }
        }

        public string Teacher
        {
            get
            {
                if (IsEmpty)
                {
                    return "";
                }
                else
                {
                    return data.Teacher;
                }
            }
        }
        public string Room
        {
            get
            {
                if (IsEmpty)
                {
                    return "";
                }
                else
                {
                    return data.Room;
                }
            }
        }

        public delegate void ContentChangedHandler(object sender, EventArgs e);
        public event ContentChangedHandler ContentChanged;

        public Group()
        {
            this.InitializeComponent();

            LoadContent();
        }
        public Group(Guid id)
        {
            this.InitializeComponent();

            this.id = id;
            data = DataHelper.GetGroupData(id);
            IsEmpty = false;

            LoadContent();
        }

        public void LoadContent()
        {
            if (IsEmpty)
            {
                if (ThemeHelper.IsDarkTheme())
                {
                    this.Icon.Foreground = new SolidColorBrush(Colors.White);
                }
                else
                {
                    this.Icon.Foreground = new SolidColorBrush(Colors.Black);
                }
            }
            else
            {
                this.Icon.Foreground = null;
                this.Background = HexToBrush(this.data.Color);
                this.Text = this.data.Name;
            }
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

        public long BrushToHex(Brush brush)
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
        public SolidColorBrush HexToBrush(long hex)
        {

            int b = (int)(hex % 0x100);
            int g = (int)(((hex - b) / 0x100) % 0x100);
            int r = (int)(((hex - b - g) / 0x10000) % 0x100);
            int a = (int)(((hex - b - g - r) / 0x1000000) % 0x100);

            SolidColorBrush brush = new SolidColorBrush(Color.FromArgb((byte)a, (byte)r, (byte)g, (byte)b));

            return brush;
        }


        async public void EditStart(object sender, RoutedEventArgs e)
        {
            dialog = new ContentDialog();

            // XamlRoot must be set in the case of a ContentDialog running in a Desktop app
            dialog.XamlRoot = this.XamlRoot;
            dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
            dialog.Title = "Edit Group";
            dialog.PrimaryButtonText = "Save";
            dialog.CloseButtonText = "Cancel";
            dialog.DefaultButton = ContentDialogButton.Primary;
            dialog.Content = new GroupEditPage(this);

            dialog.PrimaryButtonClick += EditSave;

            var result = await dialog.ShowAsync();
        }
        private void EditSave(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (IsEmpty)
            {
                id = DataHelper.CreateGroupData();
                data = DataHelper.GetGroupData(id);
            }

            this.data.Color = BrushToHex(((GroupEditPage)dialog.Content).GetColor());
            this.data.Name = ((GroupEditPage)dialog.Content).GetName();
            this.data.Teacher = ((GroupEditPage)dialog.Content).GetTeacher();
            this.data.Room = ((GroupEditPage)dialog.Content).GetRoom();

            IsEmpty = false;

            LoadContent();

            ContentChanged?.Invoke(this, null);

            DataHelper.Save(); //move
        }

        private void DeleteButtonClicked(object sender, RoutedEventArgs e)
        {
            if (!IsEmpty)
            {
                ((StackPanel)this.Parent).Children.Remove(this);
                DataHelper.RemoveGroupData(id);
                DataHelper.Save(); //move
            }
        }

        private void ContextMenuOpened(object sender, object e)
        {
            if (IsEmpty)
            {
                MenuFlyout.Hide();
            }
        }
    }
}
