using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Automation.Provider;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using System;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using Timebook.Helper;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Timebook.Controls
{

    public partial class CustomButton : Button
    {
        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            // Your custom logic here
            base.OnPointerPressed(e);

            /*
            var peer = new ButtonAutomationPeer(this);
            var invokeProvider = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
            invokeProvider?.Invoke();*/

            //ReleasePointerCapture(e.Pointer);

            e.Handled = false;
        }

        protected override void OnPointerCaptureLost(PointerRoutedEventArgs e)
        {
            //base.OnPointerCaptureLost(e);
        }
    }

    public class ClassData
    {
        public long Color { get; set; } = -0x1;
        public string Name { get; set; } = "";
        public string Teacher { get; set; } = "";
        public string Room { get; set; } = "";

        public ClassData() { }

        public ClassData(ClassData data)
        {
            this.Color = data.Color;
            this.Name = data.Name;
            this.Teacher = data.Teacher;
            this.Room = data.Room;
        }
    }

    public sealed partial class ClassButton : UserControl
    {
        ContentDialog dialog;

        Guid id;

        public bool IsEmpty = true;

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
            }
        }
        public string Text
        {
            set
            {
                this.TextBlock.Text = value;
            }
        }

        public delegate void ContentChangedHandler(object sender, EventArgs e);
        public event ContentChangedHandler ContentChanged;

        public ClassButton()
        {
            this.InitializeComponent();

            LoadContent();
            this.ActualThemeChanged += LoadContent;
        }
        public ClassButton(Guid id)
        {
            this.InitializeComponent();

            this.id = id;
            IsEmpty = false;

            LoadContent();
            this.ActualThemeChanged += LoadContent;
        }

        public void LoadContent(FrameworkElement sender = null, object e = null)
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
                this.Background = this.Background; //sets background for "+" button
            }
            else
            {
                this.Icon.Foreground = null;

                var dataTemp = DataHelper.GetClassData(id);

                this.Background = Timebook.Helper.ColorHelper.HexToBrush(dataTemp.Color);
                this.Text = dataTemp.Name;
            }
        }

        async public void EditStart(object sender, RoutedEventArgs e)
        {
            dialog = new ContentDialog();

            // XamlRoot must be set in the case of a ContentDialog running in a Desktop app
            dialog.XamlRoot = this.XamlRoot;
            dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
            dialog.Title = "Edit Class";
            dialog.PrimaryButtonText = "Save";
            dialog.CloseButtonText = "Cancel";
            dialog.DefaultButton = ContentDialogButton.Primary;

            ThemeHelper.SubscribeToThemeChange(dialog);

            if (IsEmpty)
            {
                dialog.Content = new ClassEditPage(this, new ClassData());
            }
            else
            {
                dialog.Content = new ClassEditPage(this, DataHelper.GetClassData(id));
            }

            dialog.PrimaryButtonClick += EditSave;

            var result = await dialog.ShowAsync();
        }
        private void EditSave(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (IsEmpty)
            {
                id = DataHelper.CreateClassData();
            }

            DataHelper.SetClassData(id, ((ClassEditPage)dialog.Content).GetData());


            IsEmpty = false;

            LoadContent();

            ContentChanged?.Invoke(this, null);

            DataHelper.Save(); //move to manual save when implemented
        }

        public delegate void DeletedHandler(ClassButton sender, EventArgs e);
        public event DeletedHandler Deleted;

        private void DeleteButtonClicked(object sender, RoutedEventArgs e)
        {
            if (!IsEmpty)
            {
                Deleted?.Invoke(this, null);

                DataHelper.RemoveClassData(id);
                DataHelper.Save(); //move to manual save when implemented
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
