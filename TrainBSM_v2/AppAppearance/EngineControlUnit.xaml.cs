using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Security.Cryptography.Xml;
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
using System.Windows.Threading;
using TrainBSM_v2.AppAppearance.Controls;
using TrainBSM_v2.AppAppearance.NewControls;

using static TrainBSM_v2.EngineAnalogValue.EngineAnalogValueType;

namespace TrainBSM_v2.AppAppearance
{
    /// <summary>
    /// Логика взаимодействия для EngineControlUnit.xaml
    /// </summary>
    /// 
    public partial class EngineControlUnit : UserControl
    {
        private List<IGaugeControl> _gauges = new List<IGaugeControl>();
        private List<ICounterControl> _counters = new List<ICounterControl>();

        private Random _rnd = new Random();
        private DispatcherTimer _timer;

        private Logger _logger = new Logger();
        private bool _isAnimating = false;
        private bool _isMenuOpened = false;
        private ulong counter = 0;

        public EngineControlUnit(DieselLocomotive locomotive)
        {
            InitializeComponent();
            InitializeGauges(locomotive);
            _logger.MainGrid.Background = new SolidColorBrush(Colors.Bisque);
            PanelContent.Content = _logger;

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(500);
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void _DebugRandomGenerator(IGaugeControl gauge)
        {
            if (gauge == null) return;
            gauge.Update(gauge.MinValue + _rnd.NextDouble() * (gauge.MaxValue - gauge.MinValue));
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            foreach (IGaugeControl gauge in _gauges) {
                _DebugRandomGenerator(gauge);
            }

            foreach (ICounterControl counter in _counters)
            {
                counter.Add((ulong)_rnd.Next(0, 2000));
            }
        }

        private void InitializeGauges(DieselLocomotive locomotive)
        {
            _gauges.Add(LoadAtCurrentSpeed);
            _gauges.Add(EngineRpm);
            _gauges.Add(FuelRackPosition);
            _gauges.Add(IntakeManifoldPressure);
            _gauges.Add(OilPressure);
            _gauges.Add(OilTemperature);
            _gauges.Add(CoolantTemperature);
            _gauges.Add(IntakeAirTemperature);

            _counters.Add(TotalHours);
            _counters.Add(TotalFuel);
        }

        private void ShowMenu()
        {
            if (_isAnimating) return;
            _isAnimating = true;

            var rotateAnimation = new DoubleAnimation
            {
                By = 180,
                Duration = TimeSpan.FromMilliseconds(350),
                EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut }
            };
            TextTransform.BeginAnimation(RotateTransform.AngleProperty, rotateAnimation);

            var animation = new DoubleAnimation
            {
                By = -SideMenu.ActualWidth,
                Duration = TimeSpan.FromMilliseconds(500),
                EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut }
            };
            animation.Completed += (s, e) =>
            {
                _isAnimating = false;
            };
            MenuTransform.BeginAnimation(TranslateTransform.XProperty, animation);
            ButtonTransform.BeginAnimation(TranslateTransform.XProperty, animation);

            _isMenuOpened = true;

            foreach (var msg in DieselMessagesCatalog.Messages)
            {
                _logger.AddLog(msg);
            }
        }
        private void HideMenu()
        {
            if (_isAnimating) return;
            _isAnimating = true;

            var rotateAnimation = new DoubleAnimation
            {
                By = -180,
                Duration = TimeSpan.FromMilliseconds(350),
                EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut }
            };
            TextTransform.BeginAnimation(RotateTransform.AngleProperty, rotateAnimation);

            var animation = new DoubleAnimation
            {
                By = SideMenu.ActualWidth,
                Duration = TimeSpan.FromMilliseconds(500),
                EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut }
            };
            animation.Completed += (s, e) =>
            {
                _isAnimating = false;
            };
            MenuTransform.BeginAnimation(TranslateTransform.XProperty, animation);
            ButtonTransform.BeginAnimation(TranslateTransform.XProperty, animation);

            _isMenuOpened = false;
        }
        private void MenuToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_isMenuOpened)
                ShowMenu();
            else
                HideMenu();
        }

        private void CheckAllGaugesForErrors()
        {

        }
    }
}
