using System;
using System.Windows;
using System.Windows.Controls;
using TrainBSM_v2.AppAppearance.NewControls;

namespace TrainBSM_v2.AppAppearance.NewControls
{
    public partial class RollingCounter : UserControl, ICounterControl
    {
        private RollingNumber[] _numbers;
        private ulong _maxNum = ulong.MaxValue;
        private ulong _currNum = 0;

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            nameof(Value), typeof(ulong), typeof(RollingCounter), new PropertyMetadata(0UL, OnValueChanged));

        public static readonly DependencyProperty NumbersCountProperty = DependencyProperty.Register(
            nameof(NumbersCount), typeof(int), typeof(RollingCounter), new PropertyMetadata(8, OnNumbersCountChanged));

        public static readonly DependencyProperty ResetIfOverflowProperty = DependencyProperty.Register(
            nameof(ResetIfOverflow), typeof(bool), typeof(RollingCounter), new PropertyMetadata(false));

        public static readonly DependencyProperty SensorNameProperty = DependencyProperty.Register(
            nameof(SensorName), typeof(string), typeof(RollingCounter), new PropertyMetadata("Counter", OnSensorNameChanged));

        public static readonly DependencyProperty ShowSensorNameProperty = DependencyProperty.Register(
            nameof(ShowSensorName), typeof(bool), typeof(RollingCounter), new PropertyMetadata(true, OnShowSensorNameChanged));

        public RollingCounter()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                InitNumbers(NumbersCount);
                UpdateSensorNameVisibility();
            };
        }

        public ulong Value
        {
            get => (ulong)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public string SensorName
        {
            get => (string)GetValue(SensorNameProperty);
            set => SetValue(SensorNameProperty, value);
        }

        public int NumbersCount
        {
            get => (int)GetValue(NumbersCountProperty);
            set => SetValue(NumbersCountProperty, value);
        }

        public bool ResetIfOverflow
        {
            get => (bool)GetValue(ResetIfOverflowProperty);
            set => SetValue(ResetIfOverflowProperty, value);
        }

        public bool ShowSensorName
        {
            get => (bool)GetValue(ShowSensorNameProperty);
            set => SetValue(ShowSensorNameProperty, value);
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (RollingCounter)d;
            control.SetValue((ulong)e.NewValue);
        }

        private static void OnNumbersCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (RollingCounter)d;
            if (control.DigitPanel != null)
                control.InitNumbers((int)e.NewValue);
        }

        private static void OnSensorNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (RollingCounter)d;
            if (control.NameText != null)
                control.NameText.Text = (string)e.NewValue;
        }
        private static void OnShowSensorNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (RollingCounter)d;
            control.UpdateSensorNameVisibility();
        }

        private void InitNumbers(int numbersCount)
        {
            if (DigitPanel == null) return;
            DigitPanel.Children.Clear();
            _currNum = 0;
            _maxNum = (ulong)Math.Pow(10, numbersCount) - 1;
            _numbers = new RollingNumber[numbersCount];
            for (int i = 0; i < numbersCount; i++)
            {
                var n = new RollingNumber();
                DigitPanel.Children.Add(n);
                _numbers[i] = n;
            }
            SetValue(_currNum);
        }

        private void SetValue(ulong value)
        {
            if (_numbers == null || _numbers.Length == 0) return;
            if (value > _maxNum)
            {
                if (ResetIfOverflow) value = 0;
                else value = _maxNum;
            }
            string text = value.ToString().PadLeft(_numbers.Length, '0');
            for (int i = 0; i < _numbers.Length; i++)
            {
                int num = text[i] - '0';
                _numbers[i].SetNumber(num);
            }
            _currNum = value;
        }

        private void UpdateSensorNameVisibility()
        {
            if (NameText == null || MainGrid?.RowDefinitions.Count < 2) return;

            if (ShowSensorName)
            {
                NameText.Visibility = Visibility.Visible;
                MainGrid.RowDefinitions[1].Height = GridLength.Auto;
            }
            else
            {
                NameText.Visibility = Visibility.Collapsed;
                MainGrid.RowDefinitions[1].Height = new GridLength(0);
            }
        }

        public void Update(ulong newValue)
        {
            Value = newValue;
        }

        public void Add(ulong increment)
        {
            ulong newValue = _currNum + increment;

            if (newValue < _currNum)
            {
                if (ResetIfOverflow)
                    Value = increment;
                else
                    Value = _maxNum;
            }
            else if (newValue > _maxNum)
            {
                if (ResetIfOverflow)
                    Value = newValue - _maxNum - 1;
                else
                    Value = _maxNum;
            }
            else
            {
                Value = newValue;
            }
        }

        public void Reset()
        {
            Value = 0;
        }
    }
}