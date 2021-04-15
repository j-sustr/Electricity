using System;
using System.Collections.Generic;
using System.Text;

namespace Electricity.Application.PeakDemand.Queries.GetPeakDemandOverview
{
    public class PeakDemandOverviewItem
    {
        public string GroupId { get; set; }
        public string GroupName { get; set; }

        public DateTime Month { get; set; }
        public DateTime PeakDemandTime { get; set; }
        public float PeakDemandValue { get; set; }
    }

    public class PeakDemandOverviewDto
    {
        public PeakDemandOverviewItem[] Items1 { get; set; }
        public PeakDemandOverviewItem[] Items2 { get; set; }
    }
}