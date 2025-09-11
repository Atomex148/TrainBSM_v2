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

    // -- Блока Возбуждения (БВ)
    public class ToExcitationUnitData : CanData
    {
        public double ExcitationVoltage { get; set; } 
        public double ExcitationCurrent { get; set; }
    }

    public class FromExcitationUnitData : CanData
    {
        public double ControlSignalExcitation { get; set; }
        public bool PermissioSignalExcitation { get; set; }
    }

    // -- БРУЭП
    public class ToControlUnitData : CanData
    {
        public double GeneratorVoltage { get; set; }
        public double GeneratorExcitationCurrentVoltage { get; set; }
        public double GeneratorExcitationTractionVoltage { get; set; }
        public double TEDGroup1Current { get; set; }
        public double TEDGroup2Current { get; set; }
        public double TEDGroup3Current { get; set; }
        public double ControllerPosition { get; set; }


        public bool DirectionForward { get; set; }
        public bool DirectionBackward { get; set; }
        public bool DirectionNeutral { get; set; }
        public bool ContactorZeroPos { get; set; }
        public bool DirectionSwitchZero { get;set; }
        public bool ReverserForward { get; set; }
        public bool ReverserBackward { get; set; }
        public bool ContactorTEDGroup1 { get; set; }
        public bool ContactorTEDGroup2{ get; set; }
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

    public class FromControlUnitData
    {
        public bool LeadingSection { get; set; }
        public bool TwoUnitSystem { get; set; }
        public bool Console1Active { get; set; }
        public bool Console2Active { get; set; }
        public bool IdleMode { get; set; }
        public bool EngineStartComplete { get; set; }
        public bool TractionModeAllowed { get; set; }
    }
}
