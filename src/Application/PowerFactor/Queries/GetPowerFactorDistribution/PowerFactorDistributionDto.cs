using Electricity.Application.Common.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electricity.Application.PowerFactor.Queries.GetPowerFactorDistribution
{
    public class PowerFactorDistributionItem
    {
        public BinRange Range { get; set; }

        public int? ValueMain { get; set; }
        public int? ValueL1 { get; set; }
        public int? ValueL2 { get; set; }
        public int? ValueL3 { get; set; }
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