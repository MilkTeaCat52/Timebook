using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Timebook.Helper;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Timebook.Controls
{
    public sealed partial class ClassBar : UserControl
    {
        ObservableCollection<ClassButton> Items = new ObservableCollection<ClassButton>();

        public ClassBar()
        {
            this.InitializeComponent();

            foreach (Guid key in DataHelper.GetClassOrder())
            {
                ClassButton classButton = new ClassButton(key);
                Items.Add(classButton);
                classButton.Deleted += (sender, args) => { Items.Remove(sender); };
            }
            CreateNewClassButton();
        }

        void CreateNewClassButton()
        {
            ClassButton newClassButton = new ClassButton();
            Items.Add(newClassButton);
            newClassButton.Deleted += (sender, args) => { Items.Remove(sender); };

            newClassButton.ContentChanged += OnNewClassButtonContentChanged;
        }

        void OnNewClassButtonContentChanged(object sender, EventArgs e)
        {
            ((ClassButton)sender).ContentChanged -= OnNewClassButtonContentChanged;

            CreateNewClassButton();
        }
    }
}
