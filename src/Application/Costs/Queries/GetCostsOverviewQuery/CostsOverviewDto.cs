using System;
using System.Collections.Generic;
using System.Text;

namespace Electricity.Application.Costs.Queries.GetCostsQuery
{
    public class CostsOverviewItem
    {
        public float ActiveEnergy { get; set; }

        public float ReactiveEnergy { get; set; }

        public float PeakDemand { get; set; }

        public float Cost { get; set; }
    }

    public class CostsOverviewDto
    {
        public CostsOverviewItem[] Interval1Items { get; set; }
        public CostsOverviewItem[] Interval2Items { get; set; }
    }
}