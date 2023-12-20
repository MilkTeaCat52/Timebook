using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Timebook.Helper;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Timebook.Controls
{
    public sealed partial class GroupBar : UserControl
    {
        public GroupBar()
        {
            this.InitializeComponent();

            foreach(Guid key in DataHelper.GetGroupOrder())
            {
                Group group = new Group(key);
                StackPanel.Children.Add(group);
            }
            CreateNewGroup();
        }

        void CreateNewGroup()
        {
            Group newGroup = new Group();
            StackPanel.Children.Add(newGroup);

            newGroup.ContentChanged += OnContentChanged;
        }

        void OnContentChanged(object sender, EventArgs e)
        {
            ((Group)sender).ContentChanged -= OnContentChanged;

            CreateNewGroup();
        }
    }
}
