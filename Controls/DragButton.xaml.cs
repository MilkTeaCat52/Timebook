using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Timebook.Controls
{
    public sealed partial class DragButton : UserControl
    {
        public delegate void ClickedEventHandler(object sender, EventArgs e);
        public event ClickedEventHandler Clicked;

        bool pointerDown = false;

        public string Text
        {
            get { return TextBlock.Text; }
            set { TextBlock.Text = value; }
        }

        public DragButton()
        {
            this.InitializeComponent();
            VisualStateManager.GoToState(this, "Normal", true);
        }

        public void OnPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "PointerOver", true);
        }

        public void OnPointerExited(object sender = null, PointerRoutedEventArgs e = null)
        {

            VisualStateManager.GoToState(this, "Normal", true);
            pointerDown = false;

        }

        public void OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "Pressed", true);
            pointerDown = true;
        }

        public void OnPointerReleased(object sender = null, PointerRoutedEventArgs e = null)
        {
            if (pointerDown)
            {
                VisualStateManager.GoToState(this, "PointerOver", true);
                Clicked?.Invoke(this, null);
                pointerDown = false;
            }
        }

        public void BlockClick()
        {
            pointerDown = false;
        }
    }
}
