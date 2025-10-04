using System.Windows.Media;

namespace TrainBSM_v2.AppAppearance.NewControls
{
    public interface IBaseControl
    {
        double Value { get; set; }
        double MinValue { get; set; }
        double MaxValue { get; set; }

        double? YellowZoneLow { get; set; }
        double? RedZoneLow { get; set; }
        double? YellowZoneHigh { get; set; }
        double? RedZoneHigh { get; set; }

        void Update(double newValue);
    }

}
