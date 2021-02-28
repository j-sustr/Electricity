using Electricity.Application.Common.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electricity.Application.Costs.Queries.GetCostsOverview
{
    public class RawCostsOverviewItem
    {
        public string GroupName { get; set; }

        public float[] ActiveEnergyInMonths { get; set; }

        public float[] ReactiveEnergyInMonths { get; set; }

        public float[] PeakDemandInMonths { get; set; }
    }

    public class CostsOverviewDto
    {
        public RawCostsOverviewItem[] Items1 { get; set; }
        public RawCostsOverviewItem[] Items2 { get; set; }
    }
}