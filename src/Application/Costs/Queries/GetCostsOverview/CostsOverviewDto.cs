using Electricity.Application.Common.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electricity.Application.Costs.Queries.GetCostsOverview
{
    public class CostlyQuantitiesOverviewItem
    {
        public string GroupId { get; set; }
        public string GroupName { get; set; }


        public float[] ActiveEnergyInMonths { get; set; }
        public float[] ReactiveEnergyInMonths { get; set; }
        public float[] PeakDemandInMonths { get; set; }
        public float[] CosFiInMonths { get; set; }

        public string Message { get; set; }
    }

    public class CostsOverviewDto
    {
        public CostlyQuantitiesOverviewItem[] Items1 { get; set; }
        public CostlyQuantitiesOverviewItem[] Items2 { get; set; }
    }
}