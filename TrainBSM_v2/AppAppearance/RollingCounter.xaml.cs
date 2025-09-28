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

namespace TrainBSM_v2.AppAppearance
{
    /// <summary>
    /// Логика взаимодействия для RollingCounter.xaml
    /// </summary>
    public partial class RollingCounter : UserControl
    {
        private RollingNumber[] _numbers;
        private ulong _maxNum = ulong.MaxValue;
        private ulong _currNum = 0;

        public static readonly DependencyProperty NumbersCountProperty = DependencyProperty.Register(
                nameof(NumbersCount), typeof(int), typeof(RollingCounter), new PropertyMetadata(8, OnNumbersCountChanged));

        public static readonly DependencyProperty ResetIfOverflowProperty = DependencyProperty.Register(
                nameof(ResetIfOverflow), typeof(bool), typeof(RollingCounter), new PropertyMetadata(false));

        public RollingCounter()
        {
            InitializeComponent();

            Loaded += (s, e) =>
            {
                InitNumbers(NumbersCount);
            };
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

        private static void OnNumbersCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (RollingCounter)d;
            if (control.DigitPanel != null)
                control.InitNumbers((int)e.NewValue);
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

        public void SetValue(ulong value)
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
    }
}
