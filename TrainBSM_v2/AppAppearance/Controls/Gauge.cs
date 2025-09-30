using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Animation;
using TrainBSM_v2.AppAppearance.Controls;

namespace TrainBSM_v2.AppAppearance
{
    public class Gauge
    {
        public AnalogValue analogValue;
        public double MinAngle { get; }
        public double MaxAngle { get; }
        public double Min => analogValue.Min;
        public double Max => analogValue.Max;

        private readonly Controls.GaugeControl _control;
        private readonly Func<double, Thresholds.Status> _statusEvaluator;

        public Gauge(AnalogValue value, double minAngle, double maxAngle,
            GaugeControl control, Func<double, Thresholds.Status> statusEvaluator = null)
        {
            analogValue = value;
            MinAngle = minAngle;
            MaxAngle = maxAngle;
            _control = control;
            _statusEvaluator = statusEvaluator;
        }

        public void Update(double value)
        {
            analogValue.Value = value;
            double t = (analogValue.Value - analogValue.Min) / (analogValue.Max - analogValue.Min);
            t = Math.Clamp(t, 0, 1);
            double angle = MinAngle + (MaxAngle - MinAngle) * t;
            _control.SetNeedleAngle(angle);
            _control.ChangeStatus(GetStatus());
        }

        public Thresholds.Status GetStatus()
        {
            if (_statusEvaluator != null)
                return _statusEvaluator(analogValue.Value);
            return Thresholds.Status.None;
        }
    }
}
