using Electricity.Application.Common.Services;
using Electricity.Application.Common.Utils;
using KMB.DataSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Electricity.Infrastructure.DataSource.Fake
{
    public static class FakeQuantity
    {
        readonly static double[] PAvgDistribution = new double[]
        {
            5, 15, 20
        };
        readonly static double[] CosFiDistribution = new double[]
        {
            0.99, 0.99, 0.95, 0.95, 0.95, 0.95, 0.95, 0.95, 0.95, 0.7
        };

        readonly static double[] ActiveEnergyDistribution = new double[] {
            5, 15, 20
        };
        readonly static double[] ReactiveEnergyLDistribution = new double[] {
            5, 15, 20
        };
        readonly static double[] ReactiveEnergyCDistribution = new double[]
        {
            5, 15, 20
        };

        public static RandomSeries GetMainQuantitySeries(MainQuantity quantity, int seed)
        {
            var series = new RandomSeries(seed);
            series.Cumulative = false;

            switch (quantity.Type)
            {
                case MainQuantityType.PAvg:
                    series.MaxShift = 5;
                    series.Distribution = PAvgDistribution;
                    return series;
                case MainQuantityType.CosFi:
                    series.Min = 0;
                    series.Max = 1;
                    series.MaxShift = 0.3;
                    series.Distribution = CosFiDistribution;
                    return series;
                default:
                    throw new ArgumentException("invalid quantity type");
            }
        }

        public static RandomSeries GetElectricityMeterQuantitySeries(ElectricityMeterQuantity quantity, int seed)
        {
            var series = new RandomSeries(seed);
            series.Scale = 100000;
            series.Cumulative = true;
            series.Accumulator = 0;

            switch (quantity.Type)
            {
                case ElectricityMeterQuantityType.ActiveEnergy:
                    series.MaxShift = 5;
                    series.Distribution = ActiveEnergyDistribution;
                    return series;
                case ElectricityMeterQuantityType.ReactiveEnergyL:
                    series.MaxShift = 5;
                    series.Distribution = ReactiveEnergyLDistribution;
                    return series;
                case ElectricityMeterQuantityType.ReactiveEnergyC:
                    series.MaxShift = 5;
                    series.Distribution = ReactiveEnergyCDistribution;
                    return series;
                default:
                    throw new ArgumentException("invalid quantity type");
            }
        }

        public static RandomSeries GetQuantitySeries(Quantity quantity, int seed)
        {
            if (MainQuantity.TryCreateFromQuantity(quantity, out var mainQuantity))
            {
                return GetMainQuantitySeries(mainQuantity, seed);
            }
            if (ElectricityMeterQuantity.TryCreateFromQuantity(quantity, out var emQuantity))
            {
                return GetElectricityMeterQuantitySeries(emQuantity, seed);
            }
            throw new ArgumentNullException("no quantity provided");
        }
    }
}
