using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Runtime.CompilerServices;
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
using TrainBSM_v2.AppAppearance.Controls;

namespace TrainBSM_v2.AppAppearance
{
    public class DiscreteIndicatorViewModel : INotifyPropertyChanged
    {
        private bool _isActive;

        public string Label { get; set; }

        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public partial class FromBruepUnit : UserControl
    {
        private Dictionary<GaugeControl, Gauge> _gauges = new();
        private Dictionary<string, DiscreteIndicatorViewModel> _indicatorViewModels = new();
        private DieselLocomotive _locomotive;
        private Random _rnd = new Random();
        private DispatcherTimer _timer;

        public FromBruepUnit(DieselLocomotive locomotive)
        {
            InitializeComponent();
            _locomotive = locomotive;
            InitializeGauges(locomotive);
            InitializeDiscreteIndicators(locomotive);

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

            foreach (var item in _indicatorViewModels.Values)
            {
                item.IsActive = _rnd.Next(0, 2) == 1;
            } 

        }

        private void InitializeGauges(DieselLocomotive locomotive)
        {
            _gauges[TEDGroup1Current] = new Gauge(locomotive.BRUEPAnalog.TEDGroup1Current, 0, 270, TEDGroup1Current,
                Thresholds.GetTEDStatus);
            TEDGroup1Current.Unit = locomotive.EngineAnalog.LoadAtCurrentSpeed.unitOfMeasurement ?? "";

            _gauges[TEDGroup2Current] = new Gauge(locomotive.BRUEPAnalog.TEDGroup2Current, 0, 270, TEDGroup2Current,
                Thresholds.GetTEDStatus);
            TEDGroup2Current.Unit = locomotive.EngineAnalog.FuelRackPosition.unitOfMeasurement ?? "";

            _gauges[TEDGroup3Current] = new Gauge(locomotive.BRUEPAnalog.TEDGroup3Current, 0, 270, TEDGroup3Current,
                Thresholds.GetTEDStatus);
            TEDGroup3Current.Unit = locomotive.EngineAnalog.EngineRpm.unitOfMeasurement ?? "";

            _gauges[ControllerPosition] = new Gauge(locomotive.BRUEPAnalog.ControllerPosition, 0, 270, ControllerPosition);
            ControllerPosition.Unit = locomotive.EngineAnalog.IntakeManifoldPressure.unitOfMeasurement ?? "";
        }

        private void InitializeDiscreteIndicators(DieselLocomotive locomotive)
        {
            var controlIndicators = new List<DiscreteIndicatorViewModel>
            {
                CreateIndicator("LeadingSection", "Ведущая секция"),
                CreateIndicator("TwoUnitSystem", "Система двух единиц"),
                CreateIndicator("BrakePipeBreak", "Обрыв тормозной магистрали"),
                CreateIndicator("ControlKey1", "Ключ активации пульта 1"),
                CreateIndicator("ControlKey2", "Ключ активации пульта 2"),
                CreateIndicator("EPKKey", "Ключ ЭПК"),
                CreateIndicator("AutostopBraking", "Автостопное торможение"),
                CreateIndicator("AutostopEnabled", "Включение автостопа"),
                CreateIndicator("SandSupply", "Подача песка"),
                CreateIndicator("Horn", "Тифон"),
                CreateIndicator("Whistle", "Свисток"),
                CreateIndicator("FrontCoupler", "Автосцепка передняя"),
                CreateIndicator("RearCoupler", "Автосцепка задняя"),
                CreateIndicator("SectionSelector", "Выбор секции"),
                CreateIndicator("DieselStart", "Пуск дизеля"),
                CreateIndicator("DieselStop", "Стоп дизеля"),
                CreateIndicator("IdleMode", "Холостой ход"),
                CreateIndicator("RearLeftRedLight", "Сигнальный фонарь задний левый красный"),
                CreateIndicator("RearLeftWhiteLight", "Сигнальный фонарь задний левый белый"),
                CreateIndicator("RearRightRedLight", "Сигнальный фонарь задний правый красный"),
                CreateIndicator("RearRightWhiteLight", "Сигнальный фонарь задний правый белый"),
                CreateIndicator("RearSearchlightDim", "Прожектор задний тускло"),
                CreateIndicator("RearSearchlightBright", "Прожектор задний ярко"),
            };

            var relayIndicators = new List<DiscreteIndicatorViewModel>
            {
                CreateIndicator("DieselPowerRelay", "Реле питания дизеля"),
                CreateIndicator("StarterRelay", "Реле стартера"),
                CreateIndicator("FuelPumpRelay", "Реле топливного насоса"),
                CreateIndicator("DieselStopRelay", "Реле остановки дизеля"),
                CreateIndicator("EmergencyStopRelay", "Реле экстренной остановки"),
                CreateIndicator("ProtectionRelay", "Реле защит"),
                CreateIndicator("Control1ActivationRelay", "Реле активации пульта 1"),
                CreateIndicator("Control2ActivationRelay", "Реле активации пульта 2"),
                CreateIndicator("Control1BrakeLockRelay", "Реле блокировки тормоза пульта 1"),
                CreateIndicator("Control2BrakeLockRelay", "Реле блокировки тормоза пульта 2"),
                CreateIndicator("ProtectionRelayState", "Состояние реле защит"),
                CreateIndicator("ProtectionReset", "Сброс защит"),
            };

            var safetyIndicators = new List<DiscreteIndicatorViewModel>
            {
                CreateIndicator("CompressorStartPermission", "Разрешение на запуск компрессора"),
                CreateIndicator("CompressorOilOverheat", "Датчик перегрева масла компрессора"),
                CreateIndicator("HighVoltageLock", "Блокировка ВУ"),
                CreateIndicator("VVKDoorLock", "Блокировка двери ВВК"),
                CreateIndicator("CompressorHoodDoorLock", "Блокировка двери капота компрессора"),
                CreateIndicator("AuxDrivesHoodDoorLock", "Блокировка двери капота вспомогательных приводов"),
                CreateIndicator("CoolingFaultBogie1", "Неисправность охлаждения тележка 1"),
                CreateIndicator("CoolingFaultBogie2", "Неисправность охлаждения тележка 2"),
                CreateIndicator("CoolingFaultDiesel", "Неисправность охлаждения дизеля"),
                CreateIndicator("CoolingFaultRectifier", "Неисправность охлаждения выпрямителя"),
            };

            ControlIndicatorsItemsControl.ItemsSource = controlIndicators;
            RelayIndicatorsItemsControl.ItemsSource = relayIndicators;
            SafetyIndicatorsItemsControl.ItemsSource = safetyIndicators;

            SubscribeToLocomotiveEvents(locomotive);
        }

        private DiscreteIndicatorViewModel CreateIndicator(string key, string label)
        {
            var vm = new DiscreteIndicatorViewModel { Label = label };
            _indicatorViewModels[key] = vm;
            return vm;
        }

        private void SubscribeToLocomotiveEvents(DieselLocomotive locomotive)
        {
        }

        public void SetIndicatorState(string key, bool state)
        {
            if (_indicatorViewModels.ContainsKey(key))
            {
                _indicatorViewModels[key].IsActive = state;
            }
        }

        public bool GetIndicatorState(string key)
        {
            return _indicatorViewModels.ContainsKey(key) && _indicatorViewModels[key].IsActive;
        }
    }
}
