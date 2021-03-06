using Electricity.Application.Common.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electricity.Application.PowerFactor.Queries.GetPowerFactorDistribution
{
    public class PowerFactorDistributionItem
    {
        public int Value { get; set; }

        public string Range { get; set; }

        public string Kind { get; set; }

        public int Phase { get; set; }
    }

    public class PowerFactorDistributionDto
    {
        public string GroupName { get; set; }

        public PowerFactorDistributionItem[] Distribution1 { get; set; }
        public PowerFactorDistributionItem[] Distribution2 { get; set; }

        public IntervalDto Interval1 { get; set; }
        public IntervalDto Interval2 { get; set; }
    }
}