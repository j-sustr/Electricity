using KMB.DataSource;
using Electricity.Application.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Electricity.Application.Common.Models.Quantities
{
    public enum ElectricityMeterQuantityType
    {
        ActiveEnergy,
        ReactiveEnergyL,
        ReactiveEnergyC,
    }

    public class ElectricityMeterQuantity : IEquatable<ElectricityMeterQuantity>
    {
        public ElectricityMeterQuantityType Type { get; set; }
        public Phase Phase { get; set; }

        public ElectricityMeterQuantity()
        {
        }

        public ElectricityMeterQuantity(ElectricityMeterQuantityType type, Phase phase)
        {
            Type = type;
            Phase = phase;
        }

        public bool Equals(ElectricityMeterQuantity other)
        {
            return other.Type == Type && other.Phase == Phase;
        }

        public Quantity ToQuantity()
        {
            switch (this.Type)
            {
                case ElectricityMeterQuantityType.ActiveEnergy:
                    if (Phase == Phase.Main)
                        return new Quantity("3EP", null);

                    return new Quantity($"Phase {(int)Phase}_EP{(int)Phase}", null);

                case ElectricityMeterQuantityType.ReactiveEnergyL:
                    if (Phase == Phase.Main)
                        return new Quantity("3EQL", null);

                    return new Quantity($"Phase {(int)Phase}_EQL{(int)Phase}", null);

                case ElectricityMeterQuantityType.ReactiveEnergyC:
                    if (Phase == Phase.Main)
                        return new Quantity("3EQC", null);

                    return new Quantity($"Phase {(int)Phase}_EQC{(int)Phase}", null);

                default:
                    throw new Exception("invalid quantity");
            }
        }

        public static bool TryCreateFromQuantity(Quantity quantity, out ElectricityMeterQuantity result)
        {
            result = null;

            Regex rx;
            rx = new Regex(@"3EP");
            if (rx.IsMatch(quantity.PropName))
            {
                result = new ElectricityMeterQuantity
                {
                    Type = ElectricityMeterQuantityType.ActiveEnergy,
                    Phase = Phase.Main
                };
                return true;
            }
            rx = new Regex(@"3EQL");
            if (rx.IsMatch(quantity.PropName))
            {
                result = new ElectricityMeterQuantity
                {
                    Type = ElectricityMeterQuantityType.ReactiveEnergyL,
                    Phase = Phase.Main
                };
                return true;
            }
            rx = new Regex(@"3EQC");
            if (rx.IsMatch(quantity.PropName))
            {
                result = new ElectricityMeterQuantity
                {
                    Type = ElectricityMeterQuantityType.ReactiveEnergyC,
                    Phase = Phase.Main
                };
                return true;
            }
            rx = new Regex(@"Phase (?<phase>\d)_EP\d");
            if (rx.IsMatch(quantity.PropName))
            {
                var m = rx.Match(quantity.PropName);
                result = new ElectricityMeterQuantity
                {
                    Type = ElectricityMeterQuantityType.ActiveEnergy,
                    Phase = (Phase)Int32.Parse(m.Groups["phase"].Value)
                };
                return true;
            }
            rx = new Regex(@"Phase (?<phase>\d)_EQL\d");
            if (rx.IsMatch(quantity.PropName))
            {
                var m = rx.Match(quantity.PropName);
                result = new ElectricityMeterQuantity
                {
                    Type = ElectricityMeterQuantityType.ReactiveEnergyL,
                    Phase = (Phase)Int32.Parse(m.Groups["phase"].Value)
                };
                return true;
            }
            rx = new Regex(@"Phase (?<phase>\d)_EQC\d");
            if (rx.IsMatch(quantity.PropName))
            {
                var m = rx.Match(quantity.PropName);
                result = new ElectricityMeterQuantity
                {
                    Type = ElectricityMeterQuantityType.ReactiveEnergyC,
                    Phase = (Phase)Int32.Parse(m.Groups["phase"].Value)
                };
                return true;
            }

            return false;
        }
    }
}