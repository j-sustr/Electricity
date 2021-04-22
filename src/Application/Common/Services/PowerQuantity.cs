using KMB.DataSource;
using Electricity.Application.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electricity.Application.Common.Services
{
    public enum PowerQuantityType
    {
        PAvg,
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
                case PowerQuantityType.PAvg:
                    if (Phase == Phase.Main)
                        return new Quantity("P_avg_3P", null);

                    return new Quantity($"P_avg_P{(int)Phase}", null);
            }

            throw new Exception("invalid quantity");
        }
    }
}