using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using Timebook.Helper;
using Windows.ApplicationModel.DataTransfer;
using CellID = System.Guid;
using ClassID = System.Guid;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Timebook.Controls
{
    public class CellData
    {
        public ClassID ClassDataID { get; set; } = ClassID.Empty;
        public bool OverrideColor { get; set; } = false;
        public long Color { get; set; } = -0x1;
        public bool OverrideName { get; set; } = false;
        public string Name { get; set; } = "";
        public bool OverrideTeacher { get; set; } = false;
        public string Teacher { get; set; } = "";
        public bool OverrideRoom { get; set; } = false;
        public string Room { get; set; } = "";

        public CellData() { }

        public CellData(CellData data)
        {
            this.ClassDataID = data.ClassDataID;
            this.OverrideColor = data.OverrideColor;
            this.Color = data.Color;
            this.OverrideName = data.OverrideName;
            this.Name = data.Name;
            this.OverrideTeacher = data.OverrideTeacher;
            this.Teacher = data.Teacher;
            this.OverrideRoom = data.OverrideRoom;
            this.Room = data.Room;
        }
    }

    // ALL CELLS WILL BE INITIATED WITH A CELLID BY THE TABLE CLASS
    // THE CELL CAN STILL BE EMPTY AND THIS WILL HAVE TO BE CHECKED
    public sealed partial class CellButton : UserControl
    {
        ContentDialog dialog;

        CellID id;

        new public SolidColorBrush Background
        {
            get
            {
                return DragButton.Background;
            }

            set
            {
                DragButton.Background = value;
            }
        }
        public string NameText
        {
            set
            {
                this.NameTextBlock.Text = value;
            }
        }
        public string TeacherText
        {
            set
            {
                this.TeacherTextBlock.Text = value;
            }
        }
        public string RoomText
        {
            set
            {
                this.RoomTextBlock.Text = value;
            }
        }

        public delegate void ContentChangedEventHandler(object sender, EventArgs e);
        public event ContentChangedEventHandler ContentChanged;

        public CellButton() //FOR TESTING ONLY DELETE LATER
        {
            this.InitializeComponent();

            this.id = DataHelper.CreateCellData();

            LoadContent();
            this.ActualThemeChanged += LoadContent;
        }

        public CellButton(CellID id)
        {
            this.InitializeComponent();

            this.id = id;

            LoadContent();
            this.ActualThemeChanged += LoadContent;
        }

        public void LoadContent(FrameworkElement sender = null, object e = null)
        {
            var dataTemp = DataHelper.GetCellData(id);

            SolidColorBrush background = null;
            string name = "";
            string teacher = "";
            string room = "";


            if (dataTemp.ClassDataID != ClassID.Empty)
            {
                var classDataTemp = DataHelper.GetClassData(dataTemp.ClassDataID);

                background = Timebook.Helper.ColorHelper.HexToBrush(classDataTemp.Color);
                name = classDataTemp.Name;
                teacher = classDataTemp.Teacher;
                room = classDataTemp.Room;
            }

            //overrides
            if (dataTemp.OverrideColor)
            {
                background = Timebook.Helper.ColorHelper.HexToBrush(dataTemp.Color);
            }
            if (dataTemp.OverrideName)
            {
                name = dataTemp.Name;
            }
            if (dataTemp.OverrideTeacher)
            {
                teacher = dataTemp.Teacher;
            }
            if (dataTemp.OverrideRoom)
            {
                room = dataTemp.Room;
            }

            this.Background = background;
            this.NameText = name;
            this.TeacherText = teacher;
            this.RoomText = room;
        }

        public void EditStart(object sender, EventArgs e)
        {
            dialog = new ContentDialog();

            // XamlRoot must be set in the case of a ContentDialog running in a Desktop app
            dialog.XamlRoot = this.XamlRoot;
            dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
            dialog.Title = "Edit Cell";
            dialog.PrimaryButtonText = "Save";
            dialog.CloseButtonText = "Cancel";
            dialog.DefaultButton = ContentDialogButton.Primary;
            dialog.Content = new CellEditPage();

            var result = dialog.ShowAsync();

        }

        private void EditSave(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            DataHelper.SetClassData(id, ((ClassEditPage)dialog.Content).GetData());

            LoadContent();

            ContentChanged?.Invoke(this, null);

            DataHelper.Save(); //move to manual save when implemented
        }

        private void OnDragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Copy;
        }

        private async void OnDrop(object sender, DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.Text))
            {
                string droppedItem = await e.DataView.GetTextAsync();
                ClassID classDataID = ClassID.Parse(droppedItem);

                var dataTemp = DataHelper.GetCellData(id);
                dataTemp.ClassDataID = classDataID;

                dataTemp.OverrideTeacher = false;
                dataTemp.OverrideColor = false;
                dataTemp.OverrideName = false;
                dataTemp.OverrideRoom = false;

                LoadContent();

            }
        }


        public delegate void DeletedHandler(CellButton sender, EventArgs e);
        public event DeletedHandler Deleted;
        private void DeleteButtonClicked(object sender, RoutedEventArgs e)
        {
            Deleted?.Invoke(this, null);

            DataHelper.ResetCellData(id);
            //DataHelper.Save(); //move to manual save when implemented

            LoadContent();
        }

        private void ContextMenuOpened(object sender, object e)
        {
        }
    }

}