using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace TrainBSM_v2.AppAppearance.NewControls
{
    public partial class DiscreteIndicator : UserControl
    {
        public static readonly DependencyProperty LabelTextProperty = DependencyProperty.Register(
            nameof(LabelText), typeof(string), typeof(DiscreteIndicator),
            new PropertyMetadata(string.Empty, OnLabelTextChanged));

        public static readonly DependencyProperty UseImageProperty = DependencyProperty.Register(
            nameof(UseImage), typeof(bool), typeof(DiscreteIndicator),
            new PropertyMetadata(false, OnUseImageChanged));

        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register(
            nameof(IsActive), typeof(bool), typeof(DiscreteIndicator),
            new PropertyMetadata(false, OnIsActiveChanged));

        public static readonly DependencyProperty ActiveImageSourceProperty = DependencyProperty.Register(
            nameof(ActiveImageSource), typeof(ImageSource), typeof(DiscreteIndicator),
            new PropertyMetadata(null, OnImageSourceChanged));

        public static readonly DependencyProperty InactiveImageSourceProperty = DependencyProperty.Register(
            nameof(InactiveImageSource), typeof(ImageSource), typeof(DiscreteIndicator),
            new PropertyMetadata(null, OnImageSourceChanged));

        public static readonly DependencyProperty IndicatorSizeProperty = DependencyProperty.Register(
            nameof(IndicatorSize), typeof(double), typeof(DiscreteIndicator),
            new PropertyMetadata(20.0));

        public static readonly DependencyProperty LabelMarginProperty = DependencyProperty.Register(
            nameof(LabelMargin), typeof(double), typeof(DiscreteIndicator),
            new PropertyMetadata(5.0));

        public static readonly DependencyProperty ActiveColorProperty = DependencyProperty.Register(
            nameof(ActiveColor), typeof(Brush), typeof(DiscreteIndicator),
            new PropertyMetadata(Brushes.LightGreen, OnColorChanged));

        public static readonly DependencyProperty InactiveColorProperty = DependencyProperty.Register(
            nameof(InactiveColor), typeof(Brush), typeof(DiscreteIndicator),
            new PropertyMetadata(Brushes.Green, OnColorChanged));

        public string LabelText
        {
            get => (string)GetValue(LabelTextProperty);
            set => SetValue(LabelTextProperty, value);
        }

        public bool IsActive
        {
            get => (bool)GetValue(IsActiveProperty);
            set => SetValue(IsActiveProperty, value);
        }

        public bool UseImage
        {
            get => (bool)GetValue(UseImageProperty);
            set => SetValue(UseImageProperty, value);
        }

        public double IndicatorSize
        {
            get => (double)GetValue(IndicatorSizeProperty);
            set => SetValue(IndicatorSizeProperty, value);
        }

        public ImageSource ActiveImageSource
        {
            get => (ImageSource)GetValue(ActiveImageSourceProperty);
            set => SetValue(ActiveImageSourceProperty, value);
        }

        public ImageSource InactiveImageSource
        {
            get => (ImageSource)GetValue(InactiveImageSourceProperty);
            set => SetValue(InactiveImageSourceProperty, value);
        }

        public double LabelMargin
        {
            get => (double)GetValue(LabelMarginProperty);
            set => SetValue(LabelMarginProperty, value);
        }

        public Brush ActiveColor
        {
            get => (Brush)GetValue(ActiveColorProperty);
            set => SetValue(ActiveColorProperty, value);
        }

        public Brush InactiveColor
        {
            get => (Brush)GetValue(InactiveColorProperty);
            set => SetValue(InactiveColorProperty, value);
        }

        public void ChangeActivness() => IsActive = !IsActive;
        public void ChangeActivness(bool activness) => IsActive = activness;

        private static void OnLabelTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DiscreteIndicator indicator)
            {
                indicator.Lable.Text = e.NewValue as string ?? string.Empty;
            }
        }

        private static void OnIsActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DiscreteIndicator indicator)
            {
                bool newValue = (bool)e.NewValue;
                indicator.UpdateIndicatorState(newValue);
                indicator.OnActivityChanged?.Invoke(indicator, newValue);
            }
        }

        private static void OnUseImageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DiscreteIndicator indicator)
            {
                bool useImage = (bool)e.NewValue;
                indicator.LightBulb.Visibility = useImage ? Visibility.Collapsed : Visibility.Visible;
                indicator.InactiveImage.Visibility = useImage ? Visibility.Visible : Visibility.Collapsed;
                indicator.ActiveImage.Visibility = useImage ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private static void OnImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DiscreteIndicator indicator)
            {
                if (indicator.ActiveImageSource != null || indicator.InactiveImageSource != null)
                {
                    indicator.UseImage = true;
                }
                indicator.InactiveImage.Source = indicator.InactiveImageSource;
                indicator.ActiveImage.Source = indicator.ActiveImageSource;
                indicator.UpdateIndicatorState(indicator.IsActive);
            }
        }

        private static void OnColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DiscreteIndicator indicator)
            {
                indicator.UpdateIndicatorState(indicator.IsActive);
            }
        }

        private void UpdateIndicatorState(bool isActive)
        {
            if (UseImage)
            {
                var animation = new DoubleAnimation(isActive ? 1 : 0, TimeSpan.FromMilliseconds(300));
                ActiveImage.BeginAnimation(Image.OpacityProperty, animation);
            }
            else
            {
                LightBulb.Fill = isActive ? ActiveColor : InactiveColor;
            }
        }

        public event EventHandler<bool>? OnActivityChanged;

        public DiscreteIndicator()
        {
            InitializeComponent();
        }
    }
}
