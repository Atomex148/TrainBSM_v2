using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
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
using TrainBSM_v2.AppAppearance.Controls;

namespace TrainBSM_v2.AppAppearance.NewControls
{
    public partial class SimpleGauge : UserControl, IGaugeControl
    {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            nameof(Value), typeof(double), typeof(SimpleGauge), new PropertyMetadata(0.0, OnValueChanged));

        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register(
            nameof(MinValue), typeof(double), typeof(SimpleGauge), new PropertyMetadata(0.0, OnRangeChanged));

        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register(
            nameof(MaxValue), typeof(double), typeof(SimpleGauge), new PropertyMetadata(100.0, OnRangeChanged));

        public static readonly DependencyProperty YellowZoneLowProperty = DependencyProperty.Register(
            nameof(YellowZoneLow), typeof(double?), typeof(SimpleGauge), new PropertyMetadata(null, OnZoneChanged));

        public static readonly DependencyProperty RedZoneLowProperty = DependencyProperty.Register(
            nameof(RedZoneLow), typeof(double?), typeof(SimpleGauge), new PropertyMetadata(null, OnZoneChanged));

        public static readonly DependencyProperty YellowZoneHighProperty = DependencyProperty.Register(
            nameof(YellowZoneHigh), typeof(double?), typeof(SimpleGauge), new PropertyMetadata(null, OnZoneChanged));

        public static readonly DependencyProperty RedZoneHighProperty = DependencyProperty.Register(
            nameof(RedZoneHigh), typeof(double?), typeof(SimpleGauge), new PropertyMetadata(null, OnZoneChanged));

        public static readonly DependencyProperty MajorTicksProperty = DependencyProperty.Register(
            nameof(MajorTicks), typeof(int), typeof(SimpleGauge), new PropertyMetadata(11, OnRangeChanged));

        public static readonly DependencyProperty MinorTicksProperty = DependencyProperty.Register
            (nameof(MinorTicks), typeof(int), typeof(SimpleGauge), new PropertyMetadata(3, OnRangeChanged));

        public static readonly DependencyProperty SensorNameProperty = DependencyProperty.Register(
            nameof(SensorName), typeof(string), typeof(SimpleGauge), new PropertyMetadata("", OnSensorNameChanged));

        public static readonly DependencyProperty SignVisibilityProperty = DependencyProperty.Register(
            nameof(IsSignVisible), typeof(bool), typeof(SimpleGauge), new PropertyMetadata(false, OnSignVisibilityChanged));

        public static readonly DependencyProperty ValueFontSizeProperty = DependencyProperty.Register(
            nameof(ValueFontSize), typeof(double), typeof(SimpleGauge), new PropertyMetadata(16.0, OnValueFontSizeChanged));

        public static readonly DependencyProperty LableFontSizeProperty = DependencyProperty.Register(
            nameof(LableFontSize), typeof(double), typeof(SimpleGauge), new PropertyMetadata(9.0, OnRangeChanged));

        public static readonly DependencyProperty SensorNameFontSizeProperty = DependencyProperty.Register(
            nameof(SensorNameFontSize), typeof(double), typeof(SimpleGauge), new PropertyMetadata(12.0));


        public double Value { get => (double)GetValue(ValueProperty); set => SetValue(ValueProperty, value); }
        public double MinValue { get => (double)GetValue(MinValueProperty); set => SetValue(MinValueProperty, value); }
        public double MaxValue { get => (double)GetValue(MaxValueProperty); set => SetValue(MaxValueProperty, value); }
        public double? YellowZoneLow { get => (double?)GetValue(YellowZoneLowProperty); set => SetValue(YellowZoneLowProperty, value); }
        public double? RedZoneLow { get => (double?)GetValue(RedZoneLowProperty); set => SetValue(RedZoneLowProperty, value); }
        public double? YellowZoneHigh { get => (double?)GetValue(YellowZoneHighProperty); set => SetValue(YellowZoneHighProperty, value); }
        public double? RedZoneHigh { get => (double?)GetValue(RedZoneHighProperty); set => SetValue(RedZoneHighProperty, value); }
        public bool IsSignVisible { get => (bool)GetValue(SignVisibilityProperty); set => SetValue(SignVisibilityProperty, value); }
        public double ValueFontSize { get => (double)GetValue(ValueFontSizeProperty); set => SetValue(ValueFontSizeProperty, value); }
        public double LableFontSize { get => (double)GetValue(LableFontSizeProperty); set => SetValue(LableFontSizeProperty, value); }
        public double SensorNameFontSize { get => (double)GetValue(SensorNameFontSizeProperty); set => SetValue(SensorNameFontSizeProperty, value); }

        public int MajorTicks { get => (int)GetValue(MajorTicksProperty); set => SetValue(MajorTicksProperty, value); }
        public int MinorTicks { get => (int)GetValue(MinorTicksProperty); set => SetValue(MinorTicksProperty, value); }
        public string SensorName { get => (string)GetValue(SensorNameProperty); set => SetValue(SensorNameProperty, value); }

        private const double _minAngle = 0;
        private const double _maxAngle = 260;
        private const double _centerX = 75;
        private const double _centerY = 75;
        private const double _tickRadius = 65.5;
        private const double _labelRadius = 48;
        private const double _segmentRadius = 33;
        private const double _segmentThickness = 5;

        public SimpleGauge()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                _DrawTicks();
                _DrawLabels();
                _DrawSegments();
                _UpdateNeedle(false);
                ValueSign.Visibility = IsSignVisible ? Visibility.Visible : Visibility.Collapsed;
                _UpdateValueSign(false);
            };
        }

        public void Update(double newValue)
        {
            Value = newValue;
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var gauge = (SimpleGauge)d;
            gauge._UpdateNeedle(true);
            gauge._UpdateNeedleColor(true);
            gauge._UpdateValueSign(true);
        }

        private static void OnRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var gauge = (SimpleGauge)d;
            if (gauge.IsLoaded)
            {
                gauge._DrawTicks();
                gauge._DrawLabels();
                gauge._DrawSegments();
            }
        }

        private static void OnSensorNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var gauge = (SimpleGauge)d;
            gauge.NameText.Text = (string)e.NewValue;
        }

        private static void OnZoneChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var gauge = (SimpleGauge)d;
            if (gauge.IsLoaded)
                gauge._DrawSegments();
        }

        private static void OnSignVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var gauge = (SimpleGauge)d;
            gauge.ValueSign.Visibility = gauge.IsSignVisible ? Visibility.Visible : Visibility.Collapsed;
            if (gauge.IsSignVisible)
                gauge._UpdateValueSign(true);
        }

        private static void OnValueFontSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var gauge = (SimpleGauge)d;
            gauge.ValueText.FontSize = (double)e.NewValue;
        }

        private double _MapValueToAngle(double value)
        {
            double t = (value - MinValue) / (MaxValue - MinValue);
            t = Math.Clamp(t, 0, 1);
            return _minAngle + (_maxAngle - _minAngle) * t;
        }

        private void _UpdateNeedle(bool animate)
        {
            double angle = _MapValueToAngle(Value);
            if (animate)
            {
                var anim = new DoubleAnimation
                {
                    To = angle,
                    Duration = TimeSpan.FromMilliseconds(300),
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
                };
                NeedleRotation.BeginAnimation(RotateTransform.AngleProperty, anim);
            }
            else
            {
                NeedleRotation.Angle = angle;
            }
        }
        private Color _GetStateColor()
        {
            if ((RedZoneLow.HasValue && Value < RedZoneLow.Value) ||
                (RedZoneHigh.HasValue && Value > RedZoneHigh.Value))
                return Colors.Red;

            if ((YellowZoneLow.HasValue && Value < YellowZoneLow.Value) ||
                (YellowZoneHigh.HasValue && Value > YellowZoneHigh.Value))
                return Colors.Yellow;

            return Colors.Transparent;
        }

        private Color _GetStateColor(Color defaultColor)
        {
            Color c = _GetStateColor();
            return c == Colors.Transparent ? defaultColor : c;
        }

        private void _UpdateNeedleColor(bool animate)
        {
            Color color = _GetStateColor(defaultColor: Colors.Cyan); // Явная передача аргумента для читабельности

            Color currentColor = Colors.Transparent;
            if (Needle.Fill is SolidColorBrush brush)
                currentColor = brush.Color;

            if (currentColor == color) return;

            if (animate)
            {
                var animBrush = new SolidColorBrush(currentColor);
                Needle.Fill = animBrush;
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
                Needle.Fill = new SolidColorBrush(color);
            }
        }

        private void _DrawTick(double angleDegrees, double length, double thickness, Brush brush)
        {
            double radians = angleDegrees * Math.PI / 180;
            double x1 = _centerX + _tickRadius * Math.Sin(radians);
            double y1 = _centerY + _tickRadius * Math.Cos(radians);
            double x2 = _centerX + (_tickRadius - length) * Math.Sin(radians);
            double y2 = _centerY + (_tickRadius - length) * Math.Cos(radians);
            var line = new Line { X1 = x1, Y1 = y1, X2 = x2, Y2 = y2, Stroke = brush, StrokeThickness = thickness };
            TicksCanvas.Children.Add(line);
        }

        private void _DrawTicks()
        {
            TicksCanvas.Children.Clear();
            double totalAngle = _minAngle - _maxAngle;
            for (int i = 0; i < MajorTicks; i++)
            {
                double angle = _minAngle + i * totalAngle / (MajorTicks - 1);
                _DrawTick(angle, 6, 2, Brushes.White);
            }
            if (MinorTicks > 0)
            {
                int totalTicks = (MajorTicks - 1) * (MinorTicks + 1) + 1;
                for (int i = 0; i < totalTicks; i++)
                {
                    if (i % (MinorTicks + 1) == 0) continue;
                    double angle = _minAngle + i * totalAngle / (totalTicks - 1);
                    _DrawTick(angle, 3, 1, Brushes.White);
                }
            }
        }

        private void _DrawLabels()
        {
            LabelsCanvas.Children.Clear();
            for (int i = 0; i < MajorTicks; i++)
            {
                double value = MinValue + i * (MaxValue - MinValue) / (MajorTicks - 1);
                double angle = _minAngle + i * (_maxAngle - _minAngle) / (MajorTicks - 1) + 90;
                double rad = angle * Math.PI / 180;
                double x = _centerX + _labelRadius * Math.Cos(rad);
                double y = _centerY + _labelRadius * Math.Sin(rad);

                var label = new TextBlock
                {
                    Text = value.ToString("F0"),
                    Foreground = new SolidColorBrush(Color.FromRgb(0xD4, 0xD4, 0xD4)),
                    FontSize = LableFontSize,
                    FontWeight = FontWeights.Bold
                };
                label.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                Canvas.SetLeft(label, x - label.DesiredSize.Width / 2);
                Canvas.SetTop(label, y - label.DesiredSize.Height / 2);
                LabelsCanvas.Children.Add(label);
            }
        }

        private void _DrawSegment(double startValue, double endValue, Color color)
        {
            if (startValue >= endValue) return;
            double startAngle = _MapValueToAngle(startValue) + 90;
            double endAngle = _MapValueToAngle(endValue) + 90;
            double startRad = startAngle * Math.PI / 180;
            double endRad = endAngle * Math.PI / 180;

            double x1 = _centerX + _segmentRadius * Math.Cos(startRad);
            double y1 = _centerY + _segmentRadius * Math.Sin(startRad);
            double x2 = _centerX + _segmentRadius * Math.Cos(endRad);
            double y2 = _centerY + _segmentRadius * Math.Sin(endRad);
            bool isLargeArc = Math.Abs(endAngle - startAngle) > 180;

            var path = new Path
            {
                Stroke = new SolidColorBrush(color),
                StrokeThickness = _segmentThickness,
                Data = new PathGeometry
                {
                    Figures = new PathFigureCollection
                    {
                        new PathFigure
                        {
                            StartPoint = new Point(x1, y1),
                            Segments = new PathSegmentCollection
                            {
                                new ArcSegment
                                {
                                    Point = new Point(x2, y2),
                                    Size = new Size(_segmentRadius, _segmentRadius),
                                    SweepDirection = SweepDirection.Clockwise,
                                    IsLargeArc = isLargeArc
                                }
                            }
                        }
                    }
                }
            };
            ZoneSegmentsCanvas.Children.Add(path);
        }

        private void _DrawSegments()
        {
            ZoneSegmentsCanvas.Children.Clear();
            if (RedZoneLow.HasValue)
            {
                _DrawSegment(MinValue, RedZoneLow.Value, Colors.Red);
                if (YellowZoneLow.HasValue) _DrawSegment(RedZoneLow.Value, YellowZoneLow.Value, Colors.Yellow);
            }
            else if (YellowZoneLow.HasValue)
            {
                _DrawSegment(MinValue, YellowZoneLow.Value, Colors.Yellow);
            }

            if (RedZoneHigh.HasValue)
            {
                if (YellowZoneHigh.HasValue) _DrawSegment(YellowZoneHigh.Value, RedZoneHigh.Value, Colors.Yellow);
                _DrawSegment(RedZoneHigh.Value, MaxValue, Colors.Red);
            }
            else if (YellowZoneHigh.HasValue)
            {
                _DrawSegment(YellowZoneHigh.Value, MaxValue, Colors.Yellow);
            }
        }

        private void _UpdateValueSign(bool animate)
        {
            if (!IsSignVisible) return;
            ValueText.Text = Value.ToString("F1");

            Color color = _GetStateColor(defaultColor: Colors.LimeGreen); // Явная передача аргумента для читабельности

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
