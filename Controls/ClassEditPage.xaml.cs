using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Timebook.Helper;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Timebook.Controls
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ClassEditPage : Page
    {
        ClassButton master;
        ClassData classData;

        public ClassEditPage(ClassButton master, ClassData classData)
        {
            this.InitializeComponent();

            this.master = master;
            this.classData = new ClassData(classData);
        }

        public void PageLoaded(object sender, RoutedEventArgs e)
        {
            this.ColorButton.IsEmpty = master.IsEmpty;
            this.ColorButton.Background = master.Background;

            this.ClassName = classData.Name;
            this.Teacher = classData.Teacher;
            this.Room = classData.Room;
        }

        public ClassData GetData()
        {
            classData.Color = Color;
            classData.Name = ClassName;
            classData.Teacher = Teacher;
            classData.Room = Room;


            return classData;
        }

        public long Color
        {
            get { return ColorHelper.BrushToHex(ColorButton.Background); }
            set { ColorButton.Background = ColorHelper.HexToBrush(value); }
        }

        public string ClassName
        {
            get { return ClassNameField.Text; }
            set { ClassNameField.Text = value; }
        }

        public string Teacher
        {
            get { return TeacherField.Text; }
            set {  TeacherField.Text = value; }
        }

        public string Room
        {
            get { return RoomField.Text; }
            set { RoomField.Text = value; }
        }
    }
}
