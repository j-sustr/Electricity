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
        public float CosFi { get; set; }
    }


    public class PowerFactorOverviewDto
    {
        public IList<PowerFactorOverviewItemDto> Items { get; set; } = new List<PowerFactorOverviewItemDto>();
    }
}