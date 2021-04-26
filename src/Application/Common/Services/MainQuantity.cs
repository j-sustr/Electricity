using KMB.DataSource;
using Electricity.Application.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electricity.Application.Common.Services
{
    public enum MainQuantityType
    {
        PAvg,
        CosFi
    }

    public class MainQuantity : IEquatable<MainQuantity>
    {
        public MainQuantityType Type { get; set; }
        public Phase Phase { get; set; }

        public bool Equals(MainQuantity other)
        {
            return other.Type == Type && other.Phase == Phase;
        }

        public Quantity ToQuantity()
        {
            switch (this.Type)
            {
                case MainQuantityType.PAvg:
                    if (Phase == Phase.Main)
                        return new Quantity("P_avg_3P", null);

                    return new Quantity($"P_avg_P{(int)Phase}", null);

                case MainQuantityType.CosFi:
                    if (Phase == Phase.Main)
                        return new Quantity("Cos_3Cosφ", null);

                    return new Quantity($"Cos_Cosφ{(int)Phase}", null);
            }

            throw new Exception("invalid quantity");
        }
    }
}