using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace TrainBSM_v2.AppAppearance.NewControls
{
    public partial class Thermometer : UserControl, IGaugeControl
    {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            nameof(Value), typeof(double), typeof(Thermometer), new PropertyMetadata(0.0, OnValueChanged));

        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register(
            nameof(MinValue), typeof(double), typeof(Thermometer), new PropertyMetadata(0.0, OnRangeChanged));

        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register(
            nameof(MaxValue), typeof(double), typeof(Thermometer), new PropertyMetadata(100.0, OnRangeChanged));

        public static readonly DependencyProperty YellowZoneLowProperty = DependencyProperty.Register(
            nameof(YellowZoneLow), typeof(double?), typeof(Thermometer), new PropertyMetadata(null, OnZoneChanged));

        public static readonly DependencyProperty RedZoneLowProperty = DependencyProperty.Register(
            nameof(RedZoneLow), typeof(double?), typeof(Thermometer), new PropertyMetadata(null, OnZoneChanged));

        public static readonly DependencyProperty YellowZoneHighProperty = DependencyProperty.Register(
            nameof(YellowZoneHigh), typeof(double?), typeof(Thermometer), new PropertyMetadata(null, OnZoneChanged));

        public static readonly DependencyProperty RedZoneHighProperty = DependencyProperty.Register(
            nameof(RedZoneHigh), typeof(double?), typeof(Thermometer), new PropertyMetadata(null, OnZoneChanged));

        public static readonly DependencyProperty MajorTicksProperty = DependencyProperty.Register(
            nameof(MajorTicks), typeof(int), typeof(Thermometer), new PropertyMetadata(11, OnRangeChanged));

        public static readonly DependencyProperty MinorTicksProperty = DependencyProperty.Register(
            nameof(MinorTicks), typeof(int), typeof(Thermometer), new PropertyMetadata(3, OnRangeChanged));

        public static readonly DependencyProperty SensorNameProperty = DependencyProperty.Register(
            nameof(SensorName), typeof(string), typeof(Thermometer), new PropertyMetadata("", OnSensorNameChanged));

        public static readonly DependencyProperty ValueFontSizeProperty = DependencyProperty.Register(
            nameof(ValueFontSize), typeof(double), typeof(Thermometer), new PropertyMetadata(14.0));

        public static readonly DependencyProperty SensorNameFontSizeProperty = DependencyProperty.Register(
            nameof(SensorNameFontSize), typeof(double), typeof(Thermometer), new PropertyMetadata(14.0));

        public static readonly DependencyProperty SignVisibilityProperty = DependencyProperty.Register(
            nameof(IsSignVisible), typeof(bool), typeof(Thermometer), new PropertyMetadata(true, OnSignVisibilityChanged));

        public double Value { get => (double)GetValue(ValueProperty); set => SetValue(ValueProperty, value); }
        public double MinValue { get => (double)GetValue(MinValueProperty); set => SetValue(MinValueProperty, value); }
        public double MaxValue { get => (double)GetValue(MaxValueProperty); set => SetValue(MaxValueProperty, value); }
        public double? YellowZoneLow { get => (double?)GetValue(YellowZoneLowProperty); set => SetValue(YellowZoneLowProperty, value); }
        public double? RedZoneLow { get => (double?)GetValue(RedZoneLowProperty); set => SetValue(RedZoneLowProperty, value); }
        public double? YellowZoneHigh { get => (double?)GetValue(YellowZoneHighProperty); set => SetValue(YellowZoneHighProperty, value); }
        public double? RedZoneHigh { get => (double?)GetValue(RedZoneHighProperty); set => SetValue(RedZoneHighProperty, value); }
        public int MajorTicks { get => (int)GetValue(MajorTicksProperty); set => SetValue(MajorTicksProperty, value); }
        public int MinorTicks { get => (int)GetValue(MinorTicksProperty); set => SetValue(MinorTicksProperty, value); }
        public string SensorName { get => (string)GetValue(SensorNameProperty); set => SetValue(SensorNameProperty, value); }
        public double ValueFontSize { get => (double)GetValue(ValueFontSizeProperty); set => SetValue(ValueFontSizeProperty, value); }
        public double SensorNameFontSize { get => (double)GetValue(SensorNameFontSizeProperty); set => SetValue(SensorNameFontSizeProperty, value); }
        public bool IsSignVisible { get => (bool)GetValue(SignVisibilityProperty); set => SetValue(SignVisibilityProperty, value); }

        public Thermometer()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                _DrawZones();
                _DrawTicks();
                _UpdateFill(false);
                _UpdateValueSign(false);

                NameText.Text = SensorName;

                InnerCanvas.SizeChanged += (s2, e2) =>
                {
                    _DrawZones();
                    _DrawTicks();
                    _UpdateFill(false);
                    _UpdateValueSign(false);
                };
            };
        }

        private static void OnRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var thermometer = (Thermometer)d;
            if (thermometer.IsLoaded)
            {
                thermometer._DrawZones();
                thermometer._DrawTicks();
                thermometer._UpdateFill(false);
                thermometer._UpdateValueSign(false);
            }
        }

        private static void OnZoneChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var thermometer = (Thermometer)d;
            if (thermometer.IsLoaded)
                thermometer._DrawZones();
        }

        public void Update(double newValue)
        {
            Value = newValue;
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var thermometer = (Thermometer)d;
            thermometer._UpdateFill(true);
            thermometer._UpdateValueSign(true);
        }

        private static void OnSensorNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var thermometer = (Thermometer)d;
            if (thermometer.NameText != null)
                thermometer.NameText.Text = (string)e.NewValue;
        }

        private static void OnSignVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var thermometer = (Thermometer)d;
            if (thermometer.ValueText != null)
            {
                thermometer.ValueText.Visibility = (bool)e.NewValue ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void _UpdateFill(bool animate)
        {
            if (FillRectangle == null || InnerCanvas == null) return;

            double range = MaxValue - MinValue;
            double percent = (Value - MinValue) / range;
            percent = Math.Clamp(percent, 0, 1);

            double baseHeight = 50;
            double maxHeight = InnerCanvas.ActualHeight + baseHeight;
            double targetHeight = baseHeight + (InnerCanvas.ActualHeight * percent);

            if (animate)
            {
                var anim = new DoubleAnimation
                {
                    To = targetHeight,
                    Duration = TimeSpan.FromMilliseconds(300),
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
                };
                FillRectangle.BeginAnimation(Rectangle.HeightProperty, anim);
            }
            else
            {
                FillRectangle.Height = targetHeight;
            }

            Color targetColor = _GetStateColor(defaultColor: Colors.Green);

            if (FillRectangle.Fill is SolidColorBrush currentBrush && animate)
            {
                var animBrush = new SolidColorBrush(currentBrush.Color);
                FillRectangle.Fill = animBrush;

                var colorAnim = new ColorAnimation
                {
                    To = targetColor,
                    Duration = TimeSpan.FromMilliseconds(300),
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
                };
                animBrush.BeginAnimation(SolidColorBrush.ColorProperty, colorAnim);
            }
            else
            {
                FillRectangle.Fill = new SolidColorBrush(targetColor);
            }
        }

        private Color _GetStateColor()
        {
            if ((RedZoneLow.HasValue && Value < RedZoneLow.Value) ||
                (RedZoneHigh.HasValue && Value > RedZoneHigh.Value))
                return Colors.Red;

            if ((YellowZoneLow.HasValue && Value < YellowZoneLow.Value) ||
                (YellowZoneHigh.HasValue && Value > YellowZoneHigh.Value))
                return Color.FromRgb(200, 200, 0);

            return Colors.Transparent;
        }

        private Color _GetStateColor(Color defaultColor)
        {
            Color c = _GetStateColor();
            return c == Colors.Transparent ? defaultColor : c;
        }

        private void _DrawTick(double y, double fromX, double toX, double thickness, Brush brush)
        {
            var line = new Line
            {
                X1 = fromX,
                Y1 = y,
                X2 = toX,
                Y2 = y,
                Stroke = brush,
                StrokeThickness = thickness
            };
            InnerCanvas.Children.Add(line);
        }

        private void _DrawZoneRectangle(double startValue, double endValue, Color color, double opacity = 0.3, double extraPixels = 0)
        {
            if (InnerCanvas == null) return;

            double height = InnerCanvas.ActualHeight;
            if (height == 0) return;

            double t1 = (startValue - MinValue) / (MaxValue - MinValue);
            double t2 = (endValue - MinValue) / (MaxValue - MinValue);

            t1 = Math.Clamp(t1, 0, 1);
            t2 = Math.Clamp(t2, 0, 1);

            double y1 = height * (1 - t2);
            double y2 = height * (1 - t1) + extraPixels;
            double rectHeight = y2 - y1;

            if (rectHeight <= 0) return;

            var rect = new Rectangle
            {
                Width = InnerCanvas.ActualWidth,
                Height = rectHeight,
                Fill = new SolidColorBrush(color) { Opacity = opacity }
            };

            Canvas.SetLeft(rect, 0);
            Canvas.SetTop(rect, y1);

            InnerCanvas.Children.Add(rect);
        }

        private void _DrawZones()
        {
            if (InnerCanvas == null) return;

            var toRemove = InnerCanvas.Children.OfType<Rectangle>().ToList();
            foreach (var element in toRemove)
            {
                InnerCanvas.Children.Remove(element);
            }

            bool hasRedLow = RedZoneLow.HasValue;
            bool hasYellowLow = YellowZoneLow.HasValue;

            if (hasRedLow)
            {
                _DrawZoneRectangle(MinValue, RedZoneLow.Value, Colors.Red, 0.25, extraPixels: 20);
            }

            if (hasYellowLow)
            {
                if (hasRedLow)
                {
                    _DrawZoneRectangle(RedZoneLow.Value, YellowZoneLow.Value, Colors.Yellow, 0.25);
                }
                else
                {
                    _DrawZoneRectangle(MinValue, YellowZoneLow.Value, Colors.Yellow, 0.25, extraPixels: 20);
                }
            }

            if (YellowZoneHigh.HasValue)
            {
                double endValue = RedZoneHigh.HasValue ? RedZoneHigh.Value : MaxValue;
                _DrawZoneRectangle(YellowZoneHigh.Value, endValue, Colors.Yellow, 0.25);
            }

            if (RedZoneHigh.HasValue)
                _DrawZoneRectangle(RedZoneHigh.Value, MaxValue, Colors.Red, 0.25);
        }

        private void _DrawTicks()
        {
            if (InnerCanvas == null) return;

            var toRemove = InnerCanvas.Children.OfType<Line>().ToList();
            foreach (var element in toRemove)
            {
                InnerCanvas.Children.Remove(element);
            }

            double height = InnerCanvas.ActualHeight;
            if (height == 0) return;

            double fromX = 0;
            double toX = 5;

            for (int i = 0; i < MajorTicks; i++)
            {
                double t = (double)i / (MajorTicks - 1);
                double y = height * (1 - t);
                _DrawTick(y, fromX, toX, 2, Brushes.White);
            }

            toX = 3;
            if (MinorTicks > 0)
            {
                int totalTicks = (MajorTicks - 1) * (MinorTicks + 1) + 1;
                for (int i = 0; i < totalTicks; i++)
                {
                    if (i % (MinorTicks + 1) == 0) continue;
                    double t = (double)i / (totalTicks - 1);
                    double y = height * (1 - t);
                    _DrawTick(y, fromX, toX, 1, Brushes.White);
                }
            }
        }

        private void _UpdateValueSign(bool animate)
        {
            ValueText.Text = Value.ToString("F1");

            Color color = _GetStateColor(defaultColor: Colors.LimeGreen);

            Color currentColor = Colors.Transparent;
            if (ValueText.Foreground is SolidColorBrush brush)
                currentColor = brush.Color;

            if (currentColor == color) return;

            if (animate)
            {
                var animBrush = new SolidColorBrush(currentColor);
                ValueText.Foreground = animBrush;
                var anim = new ColorAnimation
                {
                    To = color,
                    Duration = TimeSpan.FromMilliseconds(300),
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
                };
                animBrush.BeginAnimation(SolidColorBrush.ColorProperty, anim);
            }
            else
            {
                ValueText.Foreground = new SolidColorBrush(color);
            }
        }
    }
}