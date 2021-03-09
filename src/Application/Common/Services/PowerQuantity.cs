using DataSource;
using Electricity.Application.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electricity.Application.Common.Services
{
    public enum PowerQuantityType
    {
        PAvg3P,
    }

    public class PowerQuantity : IEquatable<PowerQuantity>
    {
        public PowerQuantityType Type { get; set; }
        public Phase Phase { get; set; }

        public bool Equals(PowerQuantity other)
        {
            return other.Type == Type && other.Phase == Phase;
        }

        public Quantity ToQuantity()
        {
            switch (this.Type)
            {
                case PowerQuantityType.PAvg3P:
                    return new Quantity("P_max_P3_C", "W");
            }

            throw new ArgumentException("invalid quantity");
        }
    }
}