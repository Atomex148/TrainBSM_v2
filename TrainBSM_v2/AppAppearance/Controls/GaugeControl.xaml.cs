using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TrainBSM_v2.AppAppearance.Controls
{
    /// <summary>
    /// Логика взаимодействия для GaugeControl.xaml
    /// </summary>
    public partial class GaugeControl : UserControl
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", typeof(string), typeof(GaugeControl), new PropertyMetadata("", OnTextChanged));
        public GaugeControl()
        {
            InitializeComponent();
        }

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var gauge = (GaugeControl)d;
            if (gauge.GaugeText != null) 
                gauge.GaugeText.Text = e.NewValue as string;    
        }
        public double NeedleAngle
        {
            get => ((RotateTransform)GaugeNeedle.RenderTransform).Angle;
            set => ((RotateTransform)GaugeNeedle.RenderTransform).Angle = value;
        }

        public string Unit
        {
            get => GaugeText.Text;
            set => GaugeText.Text = value;
        }

        public void ChangeStatus(Thresholds.Status status)
        {
            Color color;
            switch (status)
            {
                case Thresholds.Status.None:
                case Thresholds.Status.Normal:
                    color = Colors.Transparent;
                    break;
                case Thresholds.Status.Warning:
                    color = Colors.Gold;
                    break;
                case Thresholds.Status.Critical:
                    color = Colors.Red;
                    break;
            }

            if (!(Background is SolidColorBrush solidBrush))
            {
                solidBrush = new SolidColorBrush(Colors.Transparent);
                Background = solidBrush;
            }

            var anim = new ColorAnimation
            {
                To = color,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut }
            };

            Background.BeginAnimation(SolidColorBrush.ColorProperty, anim);
        }

        public void SetNeedleAngle(double angle) {
            var anim = new DoubleAnimation
            {
                To = angle,
                Duration = TimeSpan.FromMilliseconds(200),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };
            ((RotateTransform)GaugeNeedle.RenderTransform).BeginAnimation(RotateTransform.AngleProperty, anim);
        }
    }
}
