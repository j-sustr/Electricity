using Electricity.Application.Common.Models;
using Electricity.Application.Common.Models.Dtos;
using System.Collections.Generic;

namespace Electricity.Application.PowerFactor.Queries.GetPowerFactorOverview
{
    public class PowerFactorOverviewItem
    {
        public string GroupId { get; set; }
        public string GroupName { get; set; }
        public IntervalDto Interval { get; set; }

        public float ActiveEnergy { get; set; }
        public float ReactiveEnergyL { get; set; }
        public float ReactiveEnergyC { get; set; }
        public float CosFi { get; set; }

  
        public string Message { get; set; }
    }

    public class PowerFactorOverviewDto
    {
        public PowerFactorOverviewItem[] Items1 { get; set; }

        public PowerFactorOverviewItem[] Items2 { get; set; }
    }
}