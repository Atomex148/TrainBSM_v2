using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace TrainBSM_v2.AppAppearance.NewControls
{
    public partial class ColumnGauge : UserControl, IGaugeControl
    {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            nameof(Value), typeof(double), typeof(ColumnGauge), new PropertyMetadata(0.0, OnValueChanged));

        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register(
            nameof(MinValue), typeof(double), typeof(ColumnGauge), new PropertyMetadata(0.0, OnRangeChanged));

        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register(
            nameof(MaxValue), typeof(double), typeof(ColumnGauge), new PropertyMetadata(100.0, OnRangeChanged));

        public static readonly DependencyProperty YellowZoneLowProperty = DependencyProperty.Register(
            nameof(YellowZoneLow), typeof(double?), typeof(ColumnGauge), new PropertyMetadata(null, OnZoneChanged));

        public static readonly DependencyProperty RedZoneLowProperty = DependencyProperty.Register(
            nameof(RedZoneLow), typeof(double?), typeof(ColumnGauge), new PropertyMetadata(null, OnZoneChanged));

        public static readonly DependencyProperty YellowZoneHighProperty = DependencyProperty.Register(
            nameof(YellowZoneHigh), typeof(double?), typeof(ColumnGauge), new PropertyMetadata(null, OnZoneChanged));

        public static readonly DependencyProperty RedZoneHighProperty = DependencyProperty.Register(
            nameof(RedZoneHigh), typeof(double?), typeof(ColumnGauge), new PropertyMetadata(null, OnZoneChanged));

        public static readonly DependencyProperty MajorTicksProperty = DependencyProperty.Register(
            nameof(MajorTicks), typeof(int), typeof(ColumnGauge), new PropertyMetadata(11, OnRangeChanged));

        public static readonly DependencyProperty MinorTicksProperty = DependencyProperty.Register(
            nameof(MinorTicks), typeof(int), typeof(ColumnGauge), new PropertyMetadata(3, OnRangeChanged));

        public enum LabelSides { Left, Right }
        public static readonly DependencyProperty LabelSideProperty = DependencyProperty.Register(
            nameof(LabelSide), typeof(LabelSides), typeof(ColumnGauge), new PropertyMetadata(LabelSides.Left, OnRangeChanged));

        public double Value { get => (double)GetValue(ValueProperty); set => SetValue(ValueProperty, value); }
        public double MinValue { get => (double)GetValue(MinValueProperty); set => SetValue(MinValueProperty, value); }
        public double MaxValue { get => (double)GetValue(MaxValueProperty); set => SetValue(MaxValueProperty, value); }
        public double? YellowZoneLow { get => (double?)GetValue(YellowZoneLowProperty); set => SetValue(YellowZoneLowProperty, value); }
        public double? RedZoneLow { get => (double?)GetValue(RedZoneLowProperty); set => SetValue(RedZoneLowProperty, value); }
        public double? YellowZoneHigh { get => (double?)GetValue(YellowZoneHighProperty); set => SetValue(YellowZoneHighProperty, value); }
        public double? RedZoneHigh { get => (double?)GetValue(RedZoneHighProperty); set => SetValue(RedZoneHighProperty, value); }

        public int MajorTicks { get => (int)GetValue(MajorTicksProperty); set => SetValue(MajorTicksProperty, value); }
        public int MinorTicks { get => (int)GetValue(MinorTicksProperty); set => SetValue(MinorTicksProperty, value); }
        public LabelSides LabelSide { get => (LabelSides)GetValue(LabelSideProperty); set => SetValue(LabelSideProperty, value); }

        public ColumnGauge()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                _DrawZones();
                _DrawTicks();
                _DrawLabels();
                _UpdateFill(false);

                GlassCanvas.SizeChanged += (s2, e2) =>
                {
                    _DrawZones();
                    _DrawTicks();
                    _DrawLabels();
                    _UpdateFill(false);
                };
            };
        }

        private static void OnRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var gauge = (ColumnGauge)d;
            if (gauge.IsLoaded)
            {
                gauge._DrawZones();
                gauge._DrawTicks();
                gauge._DrawLabels();
                gauge._UpdateFill(false);
            }
        }

        private static void OnZoneChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var gauge = (ColumnGauge)d;
            if (gauge.IsLoaded)
                gauge._DrawZones();
        }

        public void Update(double newValue)
        {
            Value = newValue;
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var gauge = (ColumnGauge)d;
            gauge._UpdateFill(true);
        }

        private void _UpdateFill(bool animate)
        {
            if (FillRectangle == null || GlassCanvas == null) return;

            double range = MaxValue - MinValue;
            double percent = (Value - MinValue) / range;
            percent = Math.Clamp(percent, 0, 1);

            var transform = (ScaleTransform)FillRectangle.RenderTransform;

            if (animate)
            {
                var anim = new DoubleAnimation
                {
                    To = percent,
                    Duration = TimeSpan.FromMilliseconds(300),
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
                };
                transform.BeginAnimation(ScaleTransform.ScaleYProperty, anim);
            }
            else
            {
                transform.ScaleY = percent;
            }

            Color targetColor = GetZoneColor();

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

        private Color GetZoneColor()
        {
            if ((RedZoneLow.HasValue && Value < RedZoneLow.Value) ||
                (RedZoneHigh.HasValue && Value > RedZoneHigh.Value))
                return Colors.Red;

            if ((YellowZoneLow.HasValue && Value < YellowZoneLow.Value) ||
                (YellowZoneHigh.HasValue && Value > YellowZoneHigh.Value))
                return Color.FromRgb(200, 200, 0);

            return Colors.Green;
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
            GlassCanvas.Children.Add(line);
        }

        private void _DrawZoneRectangle(double startValue, double endValue, Color color, double opacity = 0.3)
        {
            if (GlassCanvas == null) return;

            double height = GlassCanvas.ActualHeight;
            if (height == 0) return;

            double t1 = (startValue - MinValue) / (MaxValue - MinValue);
            double t2 = (endValue - MinValue) / (MaxValue - MinValue);

            t1 = Math.Clamp(t1, 0, 1);
            t2 = Math.Clamp(t2, 0, 1);

            double y1 = height * (1 - t2);
            double y2 = height * (1 - t1);
            double rectHeight = y2 - y1;

            if (rectHeight <= 0) return;

            var rect = new Rectangle
            {
                Width = GlassCanvas.ActualWidth,
                Height = rectHeight,
                Fill = new SolidColorBrush(color) { Opacity = opacity }
            };

            Canvas.SetLeft(rect, 0);
            Canvas.SetTop(rect, y1);

            GlassCanvas.Children.Add(rect);
        }

        private void _DrawZones()
        {
            if (GlassCanvas == null) return;

            var toRemove = GlassCanvas.Children.OfType<Rectangle>().Where(r => r != FillRectangle).ToList();
            foreach (var element in toRemove)
            {
                GlassCanvas.Children.Remove(element);
            }

            if (RedZoneLow.HasValue)
                _DrawZoneRectangle(MinValue, RedZoneLow.Value, Colors.Red, 0.25);

            if (YellowZoneLow.HasValue)
            {
                double startValue = RedZoneLow.HasValue ? RedZoneLow.Value : MinValue;
                _DrawZoneRectangle(startValue, YellowZoneLow.Value, Colors.Yellow, 0.25);
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
            if (GlassCanvas == null) return;

            var toRemove = GlassCanvas.Children.OfType<Line>().ToList();
            foreach (var element in toRemove)
            {
                GlassCanvas.Children.Remove(element);
            }

            double height = GlassCanvas.ActualHeight;
            if (height == 0) return;

            double topPadding = 2;
            double bottomPadding = 2;

            double fromX = 1;
            double toX = 10;
            if (LabelSide == LabelSides.Right)
            {
                fromX = GlassCanvas.ActualWidth - 1;
                toX = GlassCanvas.ActualWidth - 10;
            }

            for (int i = 0; i < MajorTicks; i++)
            {
                double t = (double)i / (MajorTicks - 1);
                double y = bottomPadding + (height - topPadding - bottomPadding) * (1 - t);
                _DrawTick(y, fromX, toX, 2, Brushes.White);
            }

            fromX = 1;
            toX = 5;
            if (LabelSide == LabelSides.Right)
            {
                fromX = GlassCanvas.ActualWidth - 1;
                toX = GlassCanvas.ActualWidth - 5;
            }

            if (MinorTicks > 0)
            {
                int totalTicks = (MajorTicks - 1) * (MinorTicks + 1) + 1;
                for (int i = 0; i < totalTicks; i++)
                {
                    if (i % (MinorTicks + 1) == 0) continue;
                    double t = (double)i / (totalTicks - 1);
                    double y = bottomPadding + (height - topPadding - bottomPadding) * (1 - t);
                    _DrawTick(y, fromX, toX, 1, Brushes.White);
                }
            }
        }

        private void _DrawLabels()
        {
            if (GlassCanvas == null) return;
            LabelsCanvasLeft.Children.Clear();
            LabelsCanvasRight.Children.Clear();

            double height = GlassCanvas.ActualHeight;
            if (height == 0) return;

            double topPadding = 2;
            double bottomPadding = 2;

            var sideToAdd = LabelSide == LabelSides.Left ? LabelsCanvasLeft : LabelsCanvasRight;

            for (int i = 0; i < MajorTicks; i++)
            {
                double t = (double)i / (MajorTicks - 1);
                double y = bottomPadding + (height - topPadding - bottomPadding) * (1 - t);
                double value = MinValue + t * (MaxValue - MinValue);

                var label = new TextBlock
                {
                    Text = value.ToString("F0"),
                    Foreground = Brushes.Black,
                    FontSize = 10,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center
                };

                label.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

                if (LabelSide == LabelSides.Left)
                    Canvas.SetLeft(label, -label.DesiredSize.Width - 2);
                else
                    Canvas.SetLeft(label, 2);

                Canvas.SetTop(label, y - label.DesiredSize.Height / 2);
                sideToAdd.Children.Add(label);
            }
        }
    }
}