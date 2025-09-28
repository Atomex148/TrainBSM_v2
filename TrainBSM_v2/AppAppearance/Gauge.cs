using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace TrainBSM_v2.AppAppearance
{
    public class Gauge
    {
        public EngineAnalogValue engineAnalogValue;
        public double MinAngle { get; }
        public double MaxAngle { get; }
        public double Min => engineAnalogValue.Min;
        public double Max => engineAnalogValue.Max;

        private readonly GaugeControl _control;
        private readonly Func<float, Thresholds.Status> _statusEvaluator;

        public Gauge(EngineAnalogValue EngineAnalogValue, double minAngle, double maxAngle,
            GaugeControl control, Func<float, Thresholds.Status> statusEvaluator = null)
        {
            engineAnalogValue = EngineAnalogValue;
            MinAngle = minAngle;
            MaxAngle = maxAngle;
            _control = control;
            _statusEvaluator = statusEvaluator;
        }

        public void Update(double value)
        {
            double t = (value - engineAnalogValue.Min) / (engineAnalogValue.Max - engineAnalogValue.Min);
            t = Math.Clamp(t, 0, 1);
            double angle = MinAngle + (MaxAngle - MinAngle) * t;
            _control.SetNeedleAngle(angle);
            _control.ChangeStatus(GetStatus((float)value));
        }

        public Thresholds.Status GetStatus(float currentValue)
        {
            if (_statusEvaluator != null) 
                return _statusEvaluator(currentValue);
            return Thresholds.Status.None;
        }
    }
}
