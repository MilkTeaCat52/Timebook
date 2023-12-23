using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Timebook.Controls
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GroupEditPage : Page
    {
        Group master;

        public GroupEditPage(Group master)
        {
            this.InitializeComponent();

            this.master = master;
        }

        public Brush GetColor()
        {
            return ColorButton.Background;
        }

        public string GetName()
        {
            return GroupName.Text;
        }

        public string GetTeacher()
        {
            return Teacher.Text;
        }

        public string GetRoom()
        {
            return Room.Text;
        }

        public void PageLoaded(object sender, RoutedEventArgs e)
        {
            this.ColorButton.IsEmpty = master.IsEmpty;
            this.ColorButton.Background = master.Background;

            this.GroupName.Text = master.Text;

            this.Teacher.Text = master.Teacher;

            this.Room.Text = master.Room;
        }
    }
}
