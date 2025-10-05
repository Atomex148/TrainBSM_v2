using System.Windows.Media;

namespace TrainBSM_v2.AppAppearance.NewControls
{
    public interface IBaseControl<T> where T : struct
    {
        T Value { get; set; }
        void Update(T newValue);
    }

    public interface IGaugeControl : IBaseControl<double>
    {
        double MinValue { get; set; }
        double MaxValue { get; set; }
        double? YellowZoneLow { get; set; }
        double? RedZoneLow { get; set; }
        double? YellowZoneHigh { get; set; }
        double? RedZoneHigh { get; set; }
    }

    public interface ICounterControl : IBaseControl<ulong>
    {
        int NumbersCount { get; set; }
        bool ResetIfOverflow { get; set; }
        void Add(ulong increment);
        void Reset();
    }

}
