using System.Collections.Generic;

namespace Electricity.Application.PowerFactor.Queries.GetPowerFactorOverviewQuery
{
    public class PowerFactorOverviewItemDto
    {
        public string DeviceName { get; set; }
        public int Interval { get; set; }

        public float ActiveEnergy { get; set; }
        public float ReactiveEnergyL { get; set; }
        public float ReactiveEnergyC { get; set; }
        public float TanFi { get; set; }
    }


    public class PowerFactorOverviewDto
    {
        public IList<PowerFactorOverviewItemDto> Interval1Items { get; set; } = new List<PowerFactorOverviewItemDto>();
        public IList<PowerFactorOverviewItemDto> Interval2Items { get; set; } = new List<PowerFactorOverviewItemDto>();
    }
}