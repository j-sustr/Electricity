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
            throw new NotImplementedException();

            Regex rx = new Regex(@"\b(?<word>\w+)\s+(\k<word>)\b", RegexOptions.Compiled);

            var m = rx.Match(quantity.PropName);

            result = null;

            return false;
        }
    }
}