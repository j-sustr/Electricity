using DataSource;
using Electricity.Application.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electricity.Application.Common.Services
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
                        return new Quantity("3EP", "Wh");

                    return new Quantity($"EP{(int)Phase}", "Wh");

                case ElectricityMeterQuantityType.ReactiveEnergyL:
                    if (Phase == Phase.Main)
                        return new Quantity("3EQL", "varh");

                    return new Quantity($"EQL{(int)Phase}", "varh");

                case ElectricityMeterQuantityType.ReactiveEnergyC:
                    if (Phase == Phase.Main)
                        return new Quantity("3EQC", "varh");

                    return new Quantity($"EQC{(int)Phase}", "varh");

                default:
                    throw new ArgumentException("invalid quantity");
            }
        }
    }
}