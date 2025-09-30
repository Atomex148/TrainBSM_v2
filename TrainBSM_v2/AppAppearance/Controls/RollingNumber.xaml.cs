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
    /// Логика взаимодействия для RollingNumber.xaml
    /// </summary>
    public partial class RollingNumber : UserControl
    {
        private readonly TranslateTransform _translate = new TranslateTransform();
        private int _currentNumber = 0;
        const double digitHeight = 50;
        public RollingNumber()
        {
            InitializeComponent();
            DigitStack.RenderTransform = _translate;
        }

        public void SetNumber(int number)
        {
            if (number < 0 || number > 9)
                throw new ArgumentOutOfRangeException(nameof(number));
            else if (number == _currentNumber) return;

            double offset = -number * digitHeight;

            var anim = new DoubleAnimation(offset, TimeSpan.FromMilliseconds(300))
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
            };
            _translate.BeginAnimation(TranslateTransform.YProperty, anim);

            _currentNumber = number;
        }
    }
}
