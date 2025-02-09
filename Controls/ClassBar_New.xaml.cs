using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Timebook.Controls
{
    public sealed partial class ClassBar_New : UserControl
    {
        ObservableCollection<ClassButton_New> Items = new ObservableCollection<ClassButton_New>();


        public ClassBar_New()
        {
            for (int i = 0; i < 15; i++)
            {
                var r = new Random();
                Items.Add(new(r.NextInt64().ToString()));
            }

            this.InitializeComponent();
        }

        private void OnDragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            if (e.Items.Count > 0)
            {
                e.Data.SetText(((ClassButton_New)e.Items[0]).Text);  // Pass the dragged item as text
            }
        }

    }
}
