using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Timebook.Helper;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
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

        public CellID id;

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

        public CellButton(CellID id)
        {
            this.InitializeComponent();

            this.id = id;

            LoadContent();
            this.ActualThemeChanged += LoadContent;
            RefreshHelper.UIRefreshed += LoadContent;
        }

        public void LoadContent(object sender = null, object e = null)
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
            if (e.DataView.Contains("ClassID"))
            {
                e.AcceptedOperation = DataPackageOperation.Copy;
            }
            else if (e.DataView.Contains("CellID"))
            {
                e.AcceptedOperation = DataPackageOperation.Move;
                e.DragUIOverride.Caption = "\u21c4 Swap";
                e.DragUIOverride.IsGlyphVisible = false;
            }
        }

        private async void OnDrop(object sender, DragEventArgs e)
        {
            if (e.DataView.Contains("ClassID"))
            {
                string droppedItem = (await e.DataView.GetDataAsync("ClassID")).ToString();
                ClassID classDataID = ClassID.Parse(droppedItem);

                var dataTemp = DataHelper.GetCellData(id);
                dataTemp.ClassDataID = classDataID;

                dataTemp.OverrideTeacher = false;
                dataTemp.OverrideColor = false;
                dataTemp.OverrideName = false;
                dataTemp.OverrideRoom = false;
            }
            else if (e.DataView.Contains("CellID"))
            {
                string droppedItem = (await e.DataView.GetDataAsync("CellID")).ToString();
                CellID otherID = CellID.Parse(droppedItem);

                DataHelper.SwapCellData(id, otherID);                
            }
            DataHelper.Save(); //move to manual save when implemented
            LoadContent();
            RefreshHelper.RefreshUI();
        }


        public delegate void DeletedHandler(CellButton sender, EventArgs e);
        public event DeletedHandler Deleted;
        private void DeleteButtonClicked(object sender, RoutedEventArgs e)
        {
            Deleted?.Invoke(this, null);

            DataHelper.ResetCellData(id);
            DataHelper.Save(); //move to manual save when implemented

            LoadContent();
        }

        private async void OnDragStarting(UIElement sender, DragStartingEventArgs e)
        {
            DragButton.BlockClick();

            e.Data.SetData("CellID", id.ToString());
            e.Data.RequestedOperation = DataPackageOperation.Move;


            //
            var rtb = new RenderTargetBitmap();

            await rtb.RenderAsync(sender);

            IBuffer buffer = await rtb.GetPixelsAsync();

            int width = rtb.PixelWidth;
            int height = rtb.PixelHeight;

            // Create SoftwareBitmap directly from the pixel buffer
            SoftwareBitmap softwareBitmap = new SoftwareBitmap(
                BitmapPixelFormat.Bgra8,
                width,
                height,
                BitmapAlphaMode.Premultiplied);

            using (var dataReader = DataReader.FromBuffer(buffer))
            {
                byte[] pixelBytes = new byte[buffer.Length];
                dataReader.ReadBytes(pixelBytes);
                softwareBitmap.CopyFromBuffer(pixelBytes.AsBuffer());
            }//

            e.DragUI.SetContentFromSoftwareBitmap(softwareBitmap, new Point(0, 0));
            sender.Visibility = Visibility.Collapsed;

        }

        private void OnPointerReleased(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            DragButton.OnPointerReleased();
        }

        public void OnPointerExited(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            DragButton.OnPointerExited();
        }

        private void OnDropCompleted(UIElement sender, DropCompletedEventArgs args)
        {
            sender.Visibility = Visibility.Visible;
        }
    }

}