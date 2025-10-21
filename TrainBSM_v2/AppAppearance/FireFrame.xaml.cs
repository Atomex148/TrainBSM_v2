using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
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
    /// Логика взаимодействия для FireFrame.xaml
    /// </summary>
    public partial class FireFrame : UserControl
    {
        private List<DiscreteIndicator> _discreteIndicators = new List<DiscreteIndicator>();
        private Random _rnd = new Random();
        private DispatcherTimer _timer;

        public FireFrame(DieselLocomotive locomotive)
        {
            InitializeComponent();
            InitializeIndicators(locomotive);

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(500);
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            foreach (var item in _discreteIndicators)
            {
                item.IsActive = _rnd.Next(0, 2) == 1;
            }

        }

        private void InitializeIndicators(DieselLocomotive locomotive)
        {
            _discreteIndicators.Add(fire_fire_v_cab_ind);
            _discreteIndicators.Add(fire_fire_v_vvk_ind);
            _discreteIndicators.Add(fire_fire_v_kapote_privod_ind);
            _discreteIndicators.Add(fire_fire_v_kapote_comp_ind);
            _discreteIndicators.Add(fire_fire_v_kapote_dis_ind);
            _discreteIndicators.Add(fire_flt_fire_loop_ind);
        }
    }
}
