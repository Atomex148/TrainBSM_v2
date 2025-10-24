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
using System.Windows.Threading;
using TrainBSM_v2.AppAppearance.NewControls;

namespace TrainBSM_v2.AppAppearance
{
    /// <summary>
    /// Логика взаимодействия для TractionFrame.xaml
    /// </summary>
    public partial class TractionFrame : UserControl
    {
        private HashSet<IGaugeControl> _gauges = new HashSet<IGaugeControl>();
        private HashSet<ICounterControl> _counters = new HashSet<ICounterControl>();
        private HashSet<DiscreteIndicator> _indicators = new HashSet<DiscreteIndicator>();

        private Random _rnd = new Random();
        private DispatcherTimer _timer;

        public TractionFrame(DieselLocomotive locomotive)
        {
            InitializeComponent();
            InitializeGauges(locomotive);

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
                counter.Update((ulong)_rnd.Next(0, 999999));
            }

            foreach (DiscreteIndicator indicator in _indicators)
            {
                indicator.IsActive = _rnd.Next(0, 2) == 1;
            }
        }

        private void InitializeGauges(DieselLocomotive locomotive)
        {
            _gauges.Add(traction_curr_first_grp_ind);
            _gauges.Add(traction_curr_second_grp_ind);
            _gauges.Add(traction_curr_third_grp_ind);

            _counters.Add(traction_volt_trac_gen_ind);
            _counters.Add(traction_curr_exc_tg_ind);
            _counters.Add(traction_volt_exc_tg_ind);

            _indicators.Add(traction_ena_grp_ted_1_ind);
            _indicators.Add(traction_ena_grp_ted_2_ind);
            _indicators.Add(traction_ena_grp_ted_3_ind);
            _indicators.Add(traction_cont_grp_ted_1_ind);
            _indicators.Add(traction_cont_grp_ted_2_ind);
            _indicators.Add(traction_cont_grp_ted_3_ind);
            _indicators.Add(traction_cont_exc_tg_ind);
            _indicators.Add(traction_train_cont_km1_ind);
            _indicators.Add(traction_train_cont_km2_ind);
            _indicators.Add(traction_train_cont_km3_ind);
            _indicators.Add(traction_cont_fld_shunt_km4_ind);
            _indicators.Add(traction_cont_fld_shunt_km5_ind);
            _indicators.Add(traction_cont_shuntirovaniya_fld_1_ind);
            _indicators.Add(traction_cont_shuntirovaniya_fld_2_ind);
            _indicators.Add(traction_ctl_vozbuzhdeniem_ind);
            _indicators.Add(traction_ena_exc_ind);
        }
    }
}
