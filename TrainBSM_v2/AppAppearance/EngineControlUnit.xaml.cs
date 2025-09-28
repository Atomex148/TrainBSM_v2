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

using static TrainBSM_v2.EngineAnalogValue.EngineAnalogValueType;

namespace TrainBSM_v2.AppAppearance
{
    /// <summary>
    /// Логика взаимодействия для EngineControlUnit.xaml
    /// </summary>
    /// 
    public partial class EngineControlUnit : UserControl
    {

        private Dictionary<GaugeControl, Gauge> _gauges = new();
        private NumeralCounterEngineUL totalHoursCounter;
        private NumeralCounterEngineUL totalFuelCounter;
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
            InitializeNumeralCounters(locomotive);
            _logger.MainGrid.Background = new SolidColorBrush(Colors.Bisque);
            PanelContent.Content = _logger;

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(500);
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            foreach (var item in _gauges.Values)
            {
                item.Update(_rnd.Next((int)item.Min, (int)item.Max));
            }

            totalHoursCounter.Add(counter++);
            totalFuelCounter.Add(counter += 5);
        }

        private void InitializeGauges(DieselLocomotive locomotive)
        {
            _gauges[LoadAtCurrentSpeed] = new Gauge(locomotive.EngineAnalog.LoadAtCurrentSpeed, 0, 270, LoadAtCurrentSpeed);
            LoadAtCurrentSpeed.Unit = locomotive.EngineAnalog.LoadAtCurrentSpeed.unitOfMeasurement ?? "";

            _gauges[FuelRackPosition] = new Gauge(locomotive.EngineAnalog.FuelRackPosition, 0, 270, FuelRackPosition);
            FuelRackPosition.Unit = locomotive.EngineAnalog.FuelRackPosition.unitOfMeasurement ?? "";

            _gauges[EngineRpm] = new Gauge(locomotive.EngineAnalog.EngineRpm, 0, 270, EngineRpm, Thresholds.GetEngineRPMStatus);
            EngineRpm.Unit = locomotive.EngineAnalog.EngineRpm.unitOfMeasurement ?? "";

            _gauges[IntakeManifoldPressure] = new Gauge(locomotive.EngineAnalog.IntakeManifoldPressure, 0, 270, IntakeManifoldPressure);
            IntakeManifoldPressure.Unit = locomotive.EngineAnalog.IntakeManifoldPressure.unitOfMeasurement ?? "";

            _gauges[IntakeAirTemperature] = new Gauge(locomotive.EngineAnalog.IntakeAirTemperature, 0, 270, 
                IntakeAirTemperature, Thresholds.GetIntakeAirTemperatureStatus);
            IntakeAirTemperature.Unit = locomotive.EngineAnalog.IntakeAirTemperature.unitOfMeasurement ?? "";

            _gauges[CoolantTemperature] = new Gauge(locomotive.EngineAnalog.CoolantTemperature, 0, 270, 
                CoolantTemperature, Thresholds.GetCoolantTemperatureStatus);
            CoolantTemperature.Unit = locomotive.EngineAnalog.CoolantTemperature.unitOfMeasurement ?? "";

            _gauges[OilTemperature] = new Gauge(locomotive.EngineAnalog.OilTemperature, 0, 270, 
                OilTemperature, Thresholds.GetOilTemperatureStatus);
            OilTemperature.Unit = locomotive.EngineAnalog.OilTemperature.unitOfMeasurement ?? "";

            _gauges[OilPressure] = new Gauge(locomotive.EngineAnalog.OilPressure, 0, 270, 
                OilPressure, Thresholds.GetOilPressureStatus);
            OilPressure.Unit = locomotive.EngineAnalog.OilPressure.unitOfMeasurement ?? "";
        }

        private void InitializeNumeralCounters(DieselLocomotive locomotive)
        {
            totalHoursCounter = new NumeralCounterEngineUL(locomotive.EngineAnalog.TotalHours, HoursCounter);
            totalFuelCounter = new NumeralCounterEngineUL(locomotive.EngineAnalog.TotalFuel, FuelCounter);
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

    }
}
