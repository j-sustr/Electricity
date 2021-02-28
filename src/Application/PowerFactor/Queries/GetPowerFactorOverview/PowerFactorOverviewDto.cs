using Electricity.Application.Common.Models;
using System.Collections.Generic;

namespace Electricity.Application.PowerFactor.Queries.GetPowerFactorOverview
{
    public class PowerFactorOverviewItem
    {
        public string DeviceName { get; set; }

        public float ActiveEnergy { get; set; }
        public float ReactiveEnergyL { get; set; }
        public float ReactiveEnergyC { get; set; }
        public float CosFi { get; set; }

        public Interval Interval { get; set; }
    }

    public class PowerFactorOverviewIntervalData
    {
        public Interval Interval { get; set; }

        public IList<PowerFactorOverviewItem> Items { get; set; } = new List<PowerFactorOverviewItem>();
    }

    public class PowerFactorOverviewDto
    {
        public IList<PowerFactorOverviewIntervalData> Data { get; set; } = new List<PowerFactorOverviewIntervalData>();
    }
}