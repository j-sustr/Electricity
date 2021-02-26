using Electricity.Application.Common.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electricity.Application.Costs.Queries.GetCostsQuery
{
    public class CostsOverviewItem
    {
        public string GroupName { get; set; }

        public float ActiveEnergy { get; set; }

        public float ReactiveEnergy { get; set; }

        public float PeakDemand { get; set; }

        public float Cost { get; set; }

        public IntervalDto Interval { get; set; }
    }

    public class CostsOverviewDto
    {
        public CostsOverviewItem[] Items1 { get; set; }
        public CostsOverviewItem[] Items2 { get; set; }
    }
}