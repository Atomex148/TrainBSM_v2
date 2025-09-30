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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TrainBSM_v2.AppAppearance.Controls
{
    public partial class DiscreteIndicator : UserControl
    {
        public static readonly DependencyProperty LabelTextProperty = DependencyProperty.Register(nameof(LabelText),
            typeof(string), typeof(DiscreteIndicator), new PropertyMetadata(string.Empty, OnLabelTextChanged));

        private static void OnLabelTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DiscreteIndicator indicator)
            {
                indicator.Lable.Text = e.NewValue as string ?? string.Empty;
            }
        }

        public string LabelText
        {
            get => (string)GetValue(LabelTextProperty);
            set => SetValue(LabelTextProperty, value);
        }

        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register(nameof(IsActive),
            typeof(bool), typeof(DiscreteIndicator), new PropertyMetadata(false, OnIsActiveChanged));

        private static void OnIsActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DiscreteIndicator indicator)
            {
                bool newValue = (bool)e.NewValue;
                indicator.LightBulb.Fill = newValue ? Brushes.LightGreen : Brushes.Green;
                indicator.OnActivityChanged?.Invoke(indicator, newValue);
            }
        }

        public bool IsActive
        {
            get => (bool)GetValue(IsActiveProperty);
            set => SetValue(IsActiveProperty, value);
        }

        public event EventHandler<bool>? OnActivityChanged;

        public DiscreteIndicator()
        {
            InitializeComponent();
        }

        public void ChangeActivness() => IsActive = !IsActive;
        public void ChangeActivness(bool activness) => IsActive = activness;
    }
}