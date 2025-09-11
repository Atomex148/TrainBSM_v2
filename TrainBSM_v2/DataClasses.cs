using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainBSM_v2
{
    // -- Получаем по CAN данные
    public abstract class CanData
    {
        public DateTime timestamp { get; set; }
        public int CanId { get; set; }
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

        public Status GetTEDStatus(double tedCurrent)
        {
            if (tedCurrent < 950)
                return Status.Normal;
            else if (tedCurrent < 1092)
                return Status.Warning;
            return Status.Critical;
        }

        public Status GetSupplyLineStatus(double supplyPressure)
        {
            if (supplyPressure < 7.5 || supplyPressure > 8.5)
                return Status.Warning;
            return Status.Normal;
        }

        public Status GetBrakeLineStatus(double brakeLinePressure) {
            if (brakeLinePressure < 2.8)
                return Status.None;
            else if (brakeLinePressure <= 4.5)
                return Status.Normal;
            return Status.Warning;
        }

        public Status GetBrakeCylindersStatus(double brakeCylinderPressure)
        {
            if (brakeCylinderPressure < 1.5)
                return Status.None;
            else if (brakeCylinderPressure <= 4.0)
                return Status.Normal;
            return Status.Warning;
        }
    }

    // -- БРУЭП
    public class BRUEPAnalogData : CanData
    {
        public double GeneratorVoltage { get; set; }
        public double GeneratorExcitationCurrentVoltage { get; set; }
        public double GeneratorExcitationTractionVoltage { get; set; }
        public double TEDGroup1Current { get; set; }
        public double TEDGroup2Current { get; set; }
        public double TEDGroup3Current { get; set; }
        public double ControllerPosition { get; set; }

        public Thresholds Limits { get; set; } = new Thresholds();
    }


    public class BRUEPDiscreteData : CanData
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

    public class MSUDToBRUEP
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
    public class MSUDAnalogData
    {
        public double MainNetworkVoltage { get; set; }
        public double BatteryCurrent { get; set; }
        public double EngineNetworkVoltage { get; set; }
        public double BatteryEngineCurrent { get; set; }
        public double SupplyLinePressure { get; set; }
        public double BrakeLinePressure { get; set; }
        public double BrakeCylindersPressure { get; set; }
    }    

    public class MSUDDiscreteData
    {
        public bool LeadingSectionSwitch { get; set; }
    }
}
