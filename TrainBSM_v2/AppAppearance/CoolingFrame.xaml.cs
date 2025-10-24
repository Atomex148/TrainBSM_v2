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
    /// Логика взаимодействия для CoolingFrame.xaml
    /// </summary>
    public partial class CoolingFrame : UserControl
    {
        private HashSet<DiscreteIndicator> _discreteIndicators = new HashSet<DiscreteIndicator>();
        private Random _rnd = new Random();
        private DispatcherTimer _timer;
        public CoolingFrame(DieselLocomotive locomotive)
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
            _discreteIndicators.Add(cool_flt_ohlazhdeniya_bog_r_1_ind);
            _discreteIndicators.Add(cool_flt_ohlazhdeniya_bog_r_2_ind);
            _discreteIndicators.Add(cool_flt_ohlazhdeniya_dsl_r_ind);
            _discreteIndicators.Add(cool_flt_ohlazhdeniya_rect_ind);
            _discreteIndicators.Add(cool_ena_ohlazhdeniya_bog_1_ind);
            _discreteIndicators.Add(cool_ena_ohlazhdeniya_bog_2_ind);
            _discreteIndicators.Add(cool_ena_ohlazhdeniya_dsl_ind);
            _discreteIndicators.Add(cool_ena_ohlazhdeniya_rect_ind);
        }
    }
}
