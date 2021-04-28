using Electricity.Application.Common.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electricity.Application.Costs.Queries.GetCostsDetail
{
    public class CostlyQuantitiesDetailItem
    {
        public int Year { get; set; }
        public int Month { get; set; }

        public float ActiveEnergy { get; set; }
        public float ReactiveEnergy { get; set; }
        public float PeakDemand { get; set; }
        public float CosFi { get; set; }
    }

    public class CostsDetailDto
    {
        public string GroupName { get; set; }

        public CostlyQuantitiesDetailItem[] Items1 { get; set; }
        public CostlyQuantitiesDetailItem[] Items2 { get; set; }
    }
}