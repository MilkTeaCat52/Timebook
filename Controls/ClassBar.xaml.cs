using Microsoft.UI;
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
using Timebook.Helper;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;

using ClassID = System.Guid;

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

            foreach (ClassID key in DataHelper.GetClassOrder())
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

            //newClassButton.CanDrag = false;

            newClassButton.ContentChanged += OnNewClassButtonContentChanged;
        }

        void OnNewClassButtonContentChanged(object sender, EventArgs e)
        {
            ((ClassButton)sender).ContentChanged -= OnNewClassButtonContentChanged;

            CreateNewClassButton();
        }

        private void OnDragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            if (e.Items.Count > 0)
            {
                ((ClassButton)e.Items[0]).BlockClick();
                e.Data.SetText(((ClassButton)e.Items[0]).id.ToString());  // Pass the dragged item as text
            }
        }

        private void OnItemClicked(object sender, ItemClickEventArgs e)
        {
            ((ClassButton)e.ClickedItem).OnPointerReleased();
        }

        private void OnDragCompleted(object sender, DragItemsCompletedEventArgs e)
        {
            ((ClassButton)e.Items[0]).OnPointerExited();
            SaveClassOrder();
        }

        private void SaveClassOrder()
        {
            List<ClassID> classOrder = new List<ClassID>();
            foreach (ClassButton classButton in Items)
            {
                if (!classButton.IsEmpty)
                {
                    classOrder.Add(classButton.id);
                }
            }

            DataHelper.SetClassOrder(classOrder);
            DataHelper.Save(); //move to manual save when implemented
        }
    }
}
