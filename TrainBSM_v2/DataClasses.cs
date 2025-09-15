using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using TrainBSM_v2.AppAppearance;

namespace TrainBSM_v2
{
    // -- Вспомогательный класс для аналоговых значений
    //    Банально писать меньше кода, хоть и увеличит количество аллокаций, но мне кажется это незначительно
    public class AnalogValue
    {
        private float _value;
        private readonly float? _min;
        private readonly float? _max;

        public AnalogValue(float? min = null, float? max = null)
        {
            _min = min;
            _max = max;
        }

        public float Value
        {
            get => _value;
            set
            {
                if (_min.HasValue && value < _min.Value) _value = _min.Value;
                else if (_max.HasValue && value > _max.Value) _value = _max.Value;
                else _value = value;
            }
        }
    }

    // -- Получаем по CAN данные
    public abstract class CanData
    {
        public DateTime timestamp { get; set; }
        public int CanID { get; set; }
    }

    // -- Лимиты
    public class Thresholds
    {
        public enum Status
        {
            None = 0,
            Normal,
            Warning,
            Critical
        }

        public Status GetTEDStatus(float current)
        {
            if (current < 950.0)
                return Status.Normal;
            else if (current < 1092.0)
                return Status.Warning;
            return Status.Critical;
        }

        public Status GetSupplyLineStatus(float pressure)
        {
            if (pressure < 7.5 || pressure > 8.5)
                return Status.Warning;
            return Status.Normal;
        }

        public Status GetBrakeLineStatus(float pressure)
        {
            if (pressure < 2.8)
                return Status.None;
            else if (pressure <= 4.5)
                return Status.Normal;
            return Status.Warning;
        }

        public Status GetBrakeCylindersStatus(float pressure)
        {
            if (pressure < 1.5)
                return Status.None;
            else if (pressure <= 4.0)
                return Status.Normal;
            return Status.Warning;
        }

        public Status GetEngineRPMStatus(float engineRPM)
        {
            if (engineRPM >= 600.0 && engineRPM <= 1600.0)
                return Status.Normal;
            else if (engineRPM > 1600.0 && engineRPM <= 1800.0)
                return Status.Warning;
            return Status.Critical;
        }

        public Status GetIntakeAirTemperatureStatus(float temperature)
        {
            if (temperature <= 75.0)
                return Status.Normal;
            else if (temperature > 75 && temperature <= 95)
                return Status.Warning;
            return Status.Critical;
        }

        public Status GetCoolantTemperatureStatus(float temperature)
        {
            if (temperature >= 8.0 && temperature <= 105.0)
                return Status.Normal;
            else if (temperature > 105.0 && temperature <= 110.0)
                return Status.Warning;
            return Status.Critical;
        }

        public Status GetOilTemperatureStatus(float temperature)
        {
            if (temperature >= 8.0 && temperature <= 85.0)
                return Status.Normal;
            else if (temperature > 85.0 && temperature <= 90.0)
                return Status.Warning;
            return Status.Critical;
        }

        public Status GetOilPressureStatus(float pressure)
        {
            if (pressure > 0.6)
                return Status.Normal;
            if (pressure <= 0.6 && pressure >= 0.5)
                return Status.Warning;
            return Status.Critical;
        }
    }

    // -- БРУЭП
    public class BRUEPAnalogData : CanData
    {
        public AnalogValue GeneratorVoltage { get; } = new();
        public AnalogValue GeneratorExcitationCurrentVoltage { get; } = new();
        public AnalogValue GeneratorExcitationTractionVoltage { get; } = new();
        public AnalogValue TEDGroup1Current { get; } = new(0.0f, 1100.0f);
        public AnalogValue TEDGroup2Current { get; } = new(0.0f, 1100.0f);
        public AnalogValue TEDGroup3Current { get; } = new(0.0f, 1100.0f);
        public AnalogValue ControllerPosition { get; } = new(0.0f, 100.0f);
    }


    public class BRUEPDiscreteInputs : CanData
    {
        public bool DirectionForward { get; set; }
        public bool DirectionBackward { get; set; }
        public bool DirectionNeutral { get; set; }
        public bool ContactorZeroPos { get; set; }
        public bool DirectionSwitchZero { get; set; }
        public bool ReverserForward { get; set; }
        public bool ReverserBackward { get; set; }
        public bool ContactorTEDGroup1 { get; set; }
        public bool ContactorTEDGroup2 { get; set; }
        public bool ContactorTEDGroup3 { get; set; }
        public bool ShuntField1 { get; set; }
        public bool ShuntField2 { get; set; }
        public bool GeneratorExcitationContactor { get; set; }
        public bool TractionCircuitProtection { get; set; }
        public bool InsulationResistanceLow { get; set; }
        public bool InsulationProtection { get; set; }
        public bool MaxCurrentProtection { get; set; }
        public bool InsulationProtectionOn { get; set; }
        public bool MaxCurrentProtectionOn { get; set; }
        public bool TEDGroup1On { get; set; }
        public bool TEDGroup2On { get; set; }
        public bool TEDGroup3On { get; set; }
        public bool ExternalResistorMode { get; set; }
        public bool SelectiveCharacteristicMode { get; set; }
        public bool BrakeCylinderPressure { get; set; }
        public bool SandSupply { get; set; }
    }

    public class MSUDToBRUEP : CanData
    {
        public bool LeadingSection { get; set; }
        public bool TwoUnitSystem { get; set; }
        public bool Console1Active { get; set; }
        public bool Console2Active { get; set; }
        public bool IdleMode { get; set; }
        public bool EngineStartComplete { get; set; }
        public bool TractionModeAllowed { get; set; }
    }

    // -- МСДО / МСУД
    public class MSUDAnalogData : CanData
    {
        public AnalogValue MainNetworkVoltage { get; } = new(0.0f, 40.0f);
        public AnalogValue BatteryCurrent { get; } = new(-400.0f, 400.0f);
        public AnalogValue EngineNetworkVoltage { get; } = new(0.0f, 40.0f);
        public AnalogValue BatteryEngineCurrent { get; } = new(-400.0f, 400.0f);
        public AnalogValue SupplyLinePressure { get; } = new(0.0f, 10.0f);
        public AnalogValue BrakeLinePressure { get; } = new(0.0f, 10.0f);
        public AnalogValue BrakeCylindersPressure { get; } = new(0.0f, 10.0f);
    }

    public class MSUDDiscreteInputs : CanData
    {
        public bool LeadingSectionSwitch { get; set; }
        public bool TwoUnitSystem { get; set; }
        public bool BrakeLineBreak { get; set; }
        public bool EnginePowerRelay { get; set; }
        public bool StarterRelay { get; set; }
        public bool FuelPumpRelay { get; set; }
        public bool EngineStopRelay { get; set; }
        public bool EmergencyStopRelay { get; set; }
        public bool ProtectionRelay { get; set; }
        public bool Console1ActivationRelay { get; set; }
        public bool Console2ActivationRelay { get; set; }
        public bool Console1BrakeBlockRelay { get; set; }
        public bool Console2BrakeBlockRelay { get; set; }
        public bool Console1ActivationKey { get; set; }
        public bool Console2ActivationKey { get; set; }
        public bool EPKKey { get; set; }
        public bool AutoStopBraking { get; set; }
        public bool AutoStopOn { get; set; }
        public bool SandSupply { get; set; }
        public bool Horn { get; set; }
        public bool Whistle { get; set; }
        public bool CouplerFrontBtn { get; set; }
        public bool CouplerRearBtn { get; set; }
        public bool SectionSelectSwitch { get; set; }
        public bool EngineStartBtn { get; set; }
        public bool EngineStopBtn { get; set; }
        public bool IdleBtn { get; set; }
        public bool RearLeftRedLight { get; set; }
        public bool RearLeftWhiteLight { get; set; }
        public bool RearRightRedLight { get; set; }
        public bool RearRightWhiteLight { get; set; }
        public bool RearSearchlightDim { get; set; }
        public bool RearSearchlightBright { get; set; }
        public bool CompressorStartAllowed { get; set; }
        public bool CompressorOilOverheat { get; set; }
        public bool RectifierBlock { get; set; }
        public bool HVChamberDoorBlock { get; set; }
        public bool CompressorHoodBlock { get; set; }
        public bool AuxDrivesHoodBlock { get; set; }
        public bool ProtectionRelayState { get; set; }
        public bool ProtectionReset { get; set; }
        public bool CoolingFaultBogie1 { get; set; }
        public bool CoolingFaultBogie2 { get; set; }
        public bool CoolingFaultEngine { get; set; }
        public bool CoolingFaultRectifier { get; set; }
    }

    public class MSUDDiscreteOutputs : CanData
    {
        public bool ProtectionRelay { get; set; }
        public bool CompressorOn { get; set; }
        public bool EnginePowerRelay { get; set; }
        public bool StarterRelay { get; set; }
        public bool FuelPumpRelay { get; set; }
        public bool EngineStopRelay { get; set; }
        public bool EngineStartCompleteRelay { get; set; }
        public bool SandValves { get; set; }
        public bool Horn { get; set; }
        public bool Whistle { get; set; }
        public bool CouplerFront { get; set; }
        public bool CouplerRear { get; set; }
        public bool FrontLeftRedLight { get; set; }
        public bool FrontLeftWhiteLight { get; set; }
        public bool FrontRightRedLight { get; set; }
        public bool FrontRightWhiteLight { get; set; }
        public bool FrontSearchlightDim { get; set; }
        public bool FrontSearchlightBright { get; set; }
        public bool CompressorDriveOn { get; set; }
        public bool CoolingBogie1On { get; set; }
        public bool CoolingBogie2On { get; set; }
        public bool CoolingEngineOn { get; set; }
        public bool CoolingRectifierOn { get; set; }
    }

    // -- Двигатель

    // -- Вспомогательный класс уже для аналогового значения двигателя
    //    Добавляется же еще SPN (Suspect Parameter Number), он же уникальный номер параметра
    public class EngineAnalogValue : AnalogValue
    {
        public ushort SPN { get; }

        public EngineAnalogValue(ushort spn, float? min = null, float? max = null) : base(min, max)
        {
            SPN = spn;
        }
    }

    public class EngineAnalogValueUL
    {
        public ushort SPN { get; }
        public ulong Value { get; set; }

        public EngineAnalogValueUL(ushort spn, ulong value = 0)
        {
            SPN = spn;
            Value = value;
        }
    }

    public class EngineAnalogData : CanData
    {
        public EngineAnalogValue LoadAtCurrentSpeed { get; } = new(92, -125, 125);
        public EngineAnalogValue FuelRackPosition { get; } = new(1210, 0, 100);
        public EngineAnalogValue EngineRpm { get; } = new(190, 0, 2000);
        public EngineAnalogValue IntakeManifoldPressure { get; } = new(102, 0, 5);
        public EngineAnalogValue IntakeAirTemperature { get; set; } = new(105, -40, 100);
        public EngineAnalogValue CoolantTemperature { get; } = new(110, 0, 120);
        public EngineAnalogValue OilTemperature { get; } = new(175, 0, 120);
        public EngineAnalogValue OilPressure { get; } = new(100, 0, 20);
        public EngineAnalogValueUL TotalHours { get; } = new(247);
        public EngineAnalogValueUL TotalFuel { get; } = new(250);
    }

    // -- Ошибки двигателя

    public class EngineFmi
    {
        [JsonPropertyName("FMI")]
        public ushort FMI { get; set; }

        [JsonPropertyName("FMIDescription")]
        public string Description { get; set; }

        public EngineFmi(ushort fmi, string description)
        {
            FMI = fmi;
            Description = description;
        }
        public EngineFmi() {}
    }

    public class EngineFaultSpn
    {
        [JsonPropertyName("SPN")]
        public ushort SPN { get; set; }

        [JsonPropertyName("SPNDescription")]
        public string Description { get; set; }

        [JsonPropertyName("FMIs")]
        public List<EngineFmi> Fmis { get; set; }

        public EngineFaultSpn(ushort spn, string description, List<EngineFmi> fmis)
        {
            SPN = spn;
            Description = description;
            Fmis = fmis;
        }

        public EngineFaultSpn() {}
    }

    public static class EngineFaultCatalog
    {
        public static readonly Dictionary<ushort, EngineFaultSpn> Faults;

        static EngineFaultCatalog() {
            try
            {
                // Потом подправлю абсолютный путь на относительный
                string json = File.ReadAllText("D:\\VS projects\\TrainBSM_v2\\TrainBSM_v2\\engine_faults.json");
                var dict = JsonSerializer.Deserialize<Dictionary<ushort, EngineFaultSpn>>(json);

                Faults = dict ?? new Dictionary<ushort, EngineFaultSpn>();
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("Файл engine_faults.json не найден. Используется пустой справочник.",
                                "Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                Faults = new Dictionary<ushort, EngineFaultSpn>();
            }
            catch (JsonException ex)
            {
                MessageBox.Show($"Ошибка парсинга JSON: {ex.Message}",
                                "Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                Faults = new Dictionary<ushort, EngineFaultSpn>();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Неожиданная ошибка при загрузке JSON: {ex.Message}",
                                "Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                Faults = new Dictionary<ushort, EngineFaultSpn>();
            }
        }
    }

    public class DieselMessage
    {
        [JsonPropertyName("Id")]
        public int Id { get; set; }

        [JsonPropertyName("Message")]
        public required string Message { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public LoggerMessageType DisplayType { get; set; } = LoggerMessageType.Normal;

        [JsonPropertyName("Code")]
        public string? Code { get; set; }
    }
    public static class DieselMessagesCatalog
    {
        public static readonly List<DieselMessage> Messages;

        static DieselMessagesCatalog()
        {
            try
            {
                // Потом подправлю абсолютный путь на относительный
                string json = File.ReadAllText("D:\\VS projects\\TrainBSM_v2\\TrainBSM_v2\\diesel_faults.json");
                var messages = JsonSerializer.Deserialize<List<DieselMessage>>(json);

                Messages = messages ?? new List<DieselMessage>();
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("Файл diesel_messages.json не найден. Используется пустой список сообщений.",
                                "Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                Messages = new List<DieselMessage>();
            }
            catch (JsonException ex)
            {
                MessageBox.Show($"Ошибка парсинга JSON: {ex.Message}",
                                "Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                Messages = new List<DieselMessage>();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Неожиданная ошибка при загрузке JSON: {ex.Message}",
                                "Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                Messages = new List<DieselMessage>();
            }
        }
    }
}
