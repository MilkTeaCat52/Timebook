using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Timebook.Controls
{
    public sealed partial class ClassButton_New : UserControl
    {
        public Guid id;

        public string Text
        {
            get { return DragButton.Text; }
            set { DragButton.Text = value; }
        }

        public ClassButton_New()
        {
            this.InitializeComponent();
        }

        public ClassButton_New(Guid id)
        {
            this.InitializeComponent();
            this.id = id;
        }

        public void OnPointerReleased()
        {
            DragButton.OnPointerReleased();
        }

        public void OnPointerExited()
        {
            DragButton.OnPointerExited();
        }

        public void Clicked()
        {
            DragButton.Text = "Chungus";
        }
    }
}
