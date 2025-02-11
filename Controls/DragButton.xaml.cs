using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using System;
using Windows.UI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Timebook.Controls
{
    [ContentProperty(Name = "AdditionalContent")]
    public sealed partial class DragButton : UserControl
    {
        public object AdditionalContent
        {
            get { return (object)GetValue(AdditionalContentProperty); }
            set { SetValue(AdditionalContentProperty, value); }
        }
        public static readonly DependencyProperty AdditionalContentProperty =
            DependencyProperty.Register("AdditionalContent", typeof(object), typeof(DragButton), new PropertyMetadata(null));

        public delegate void ClickedEventHandler(object sender, EventArgs e);
        public event ClickedEventHandler Clicked;

        bool pointerDown = false;

        public SolidColorBrush _background;
        new public SolidColorBrush Background
        {
            get
            {
                return _background;
            }
            set
            {
                var color = value.Color;

                UpdateStoryboardResource("NormalStateStoryboard", value);
                UpdateStoryboardResource("PointerOverStateStoryboard", Helper.ColorHelper.GetButtonHoverBrush(color));
                UpdateStoryboardResource("PressedStateStoryboard", Helper.ColorHelper.GetButtonPressedBrush(color));

                //Refresh Visuals
                VisualStateManager.GoToState(this, "PointerOver", true);
                VisualStateManager.GoToState(this, "Normal", true);

                _background = value;
            }
        }

        void UpdateStoryboardResource(string storyboardName, Brush brush)
        {
            var storyboard = (Storyboard)ContentPresenter.FindName(storyboardName);
            var backgroundAnimation = (ObjectAnimationUsingKeyFrames)storyboard.Children[0]; // Get the first animation in the storyboard
            var keyFrame = (DiscreteObjectKeyFrame)backgroundAnimation.KeyFrames[0]; // Access the first keyframe

            // Update the animation's value with the new resource
            keyFrame.Value = brush;
        }

        public DragButton()
        {
            this.InitializeComponent();

            Background = new SolidColorBrush(Color.FromArgb(0x0f, 0xff, 0xff, 0xff));

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
            PointerPoint point = e.GetCurrentPoint(this);
            if (point.Properties.IsLeftButtonPressed)
            {
                VisualStateManager.GoToState(this, "Pressed", true);
                pointerDown = true;
            }
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
