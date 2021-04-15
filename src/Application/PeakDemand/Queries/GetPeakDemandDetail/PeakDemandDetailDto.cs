using System;
using System.Collections.Generic;
using System.Text;

namespace Electricity.Application.PeakDemand.Queries.GetPeakDemandDetail
{
    public class PeakDemandDetailData
    {
        public object[][] DemandSeries { get; set; }
    }

    public class PeakDemandDetailDto
    {
        public PeakDemandDetailData Data1 { get; set; }
        public PeakDemandDetailData Data2 { get; set; }
    }
}
