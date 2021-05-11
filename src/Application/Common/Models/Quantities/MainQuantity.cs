using KMB.DataSource;
using Electricity.Application.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Electricity.Application.Common.Models.Quantities
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

        public MainQuantity()
        {

        }

        public MainQuantity(MainQuantityType type, Phase phase)
        {
            Type = type;
            Phase = phase;
        }

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

        public static bool TryCreateFromQuantity(Quantity quantity, out MainQuantity result)
        {
            result = null;

            Regex rx;
            rx = new Regex(@"P_avg_3P");
            if (rx.IsMatch(quantity.PropName))
            {
                result = new MainQuantity
                {
                    Type = MainQuantityType.PAvg,
                    Phase = Phase.Main
                };
                return true;
            }
            rx = new Regex(@"Cos_3Cosφ");
            if (rx.IsMatch(quantity.PropName))
            {
                result = new MainQuantity
                {
                    Type = MainQuantityType.CosFi,
                    Phase = Phase.Main
                };
                return true;
            }
            rx = new Regex(@"P_avg_P(?<phase>\d)");
            if (rx.IsMatch(quantity.PropName))
            {
                var m = rx.Match(quantity.PropName);
                result = new MainQuantity
                {
                    Type = MainQuantityType.PAvg,
                    Phase = (Phase)Int32.Parse(m.Groups["phase"].Value)
                };
                return true;
            }
            rx = new Regex(@"Cos_Cosφ(?<phase>\d)");
            if (rx.IsMatch(quantity.PropName))
            {
                var m = rx.Match(quantity.PropName);
                result = new MainQuantity
                {
                    Type = MainQuantityType.CosFi,
                    Phase = (Phase)Int32.Parse(m.Groups["phase"].Value)
                };
                return true;
            }


            return false;
        }
    }
}