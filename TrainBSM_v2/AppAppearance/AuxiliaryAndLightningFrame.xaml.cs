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
    public partial class AuxiliaryAndLightningFrame : UserControl
    {
        private DispatcherTimer _sandValveTimer;
        public AuxiliaryAndLightningFrame(DieselLocomotive locomotive)
        {
            InitializeComponent();

            _sandValveTimer = new DispatcherTimer();
            _sandValveTimer.Interval = TimeSpan.FromMilliseconds(250);
            _sandValveTimer.Tick += (s, e) => aux_valv_sand.ChangeActivness();

            aux_podacha_sand.OnActivityChanged += (sender, isActive) =>
            {
                if (isActive)
                {
                    _sandValveTimer.Start();
                }
                else
                {
                    _sandValveTimer.Stop();
                    aux_valv_sand.ChangeActivness(false);
                }
            };

            SetupMutuallyExclusive(light_lamp_rr_l_red, light_lamp_rr_l_white);
            SetupMutuallyExclusive(light_lamp_rr_r_red, light_lamp_rr_r_white);
            SetupMutuallyExclusive(light_lamp_fr_l_red, light_lamp_fr_l_white);
            SetupMutuallyExclusive(light_lamp_fr_r_red, light_lamp_fr_r_white);
            SetupMutuallyExclusive(light_hl_rr_lo, light_hl_rr_hi);
            SetupMutuallyExclusive(light_hl_fr_lo, light_hl_fr_hi);
        }

        private void SetupMutuallyExclusive(DiscreteIndicator first, DiscreteIndicator second)
        {
            first.OnActivityChanged += (sender, isActive) =>
            {
                if (isActive) second.ChangeActivness(false);
            };

            second.OnActivityChanged += (sender, isActive) =>
            {
                if (isActive) first.ChangeActivness(false);
            };
        }
    }
}
