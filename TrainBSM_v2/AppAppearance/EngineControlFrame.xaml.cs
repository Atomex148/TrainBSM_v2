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
using TrainBSM_v2.AppAppearance.NewControls;

using static TrainBSM_v2.EngineAnalogValue.EngineAnalogValueType;

namespace TrainBSM_v2.AppAppearance
{
    /// <summary>
    /// Логика взаимодействия для EngineControlUnit.xaml
    /// </summary>
    /// 
    public partial class EngineControlFrame : UserControl
    {
        private HashSet<IGaugeControl> _gauges = new HashSet<IGaugeControl>();
        private HashSet<ICounterControl> _counters = new HashSet<ICounterControl>();
        private HashSet<DiscreteIndicator> _indicators = new HashSet<DiscreteIndicator>();

        private Random _rnd = new Random();
        private DispatcherTimer _timer;

        private Logger _logger = new Logger();
        private bool _isAnimating = false;
        private bool _isMenuOpened = false;
        private ulong counter = 0;

        public EngineControlFrame(DieselLocomotive locomotive)
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
            foreach (IGaugeControl gauge in _gauges)
            {
                _DebugRandomGenerator(gauge);
            }

            foreach (ICounterControl counter in _counters)
            {
                counter.Add((ulong)_rnd.Next(0, 2000));
            }

            foreach (DiscreteIndicator indicator in _indicators)
            {
                indicator.IsActive = _rnd.Next(0, 2) == 1;
            }
        }

        private void InitializeGauges(DieselLocomotive locomotive)
        {
            _gauges.Add(engine_pos_rack_injection_ind);
            _gauges.Add(engine_load_na_curr_ind);
            _gauges.Add(engine_freq_rot_crank_ind);
            _gauges.Add(engine_temp_air_vo_ind);
            _gauges.Add(engine_temp_cool_ind);
            _gauges.Add(engine_temp_oil_ind);
            _gauges.Add(engine_press_air_vo_ind);
            _gauges.Add(engine_press_oil_ind);

            _counters.Add(engine_obschee_cons_fuel_ind);
            _counters.Add(engine_tot_mile_ind);

            _indicators.Add(engine_rl_pwr_dsl_ind);
            _indicators.Add(engine_rl_startera_ind);
            _indicators.Add(engine_rl_fuel_pump_ind);
            _indicators.Add(engine_rl_stop_dsl_ind);
            _indicators.Add(engine_btn_start_dsl_ind);
            _indicators.Add(engine_btn_stop_dsl_ind);
            _indicators.Add(engine_flag_end_puska_ind);
            _indicators.Add(engine_ena_na_start_ind);
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
