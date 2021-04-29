using Electricity.Application.Common.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electricity.Application.PeakDemand.Queries.GetPeakDemandOverview
{
    public class PeakDemandOverviewItem
    {
        public string GroupId { get; set; }
        public string GroupName { get; set; }
        public IntervalDto Interval { get; set; }

        public PeakDemandItemDto[] PeakDemands { get; set; }

        public string Message { get; set; }
    }

    public class PeakDemandOverviewDto
    {
        public PeakDemandOverviewItem[] Items1 { get; set; }
        public PeakDemandOverviewItem[] Items2 { get; set; }
    }
}