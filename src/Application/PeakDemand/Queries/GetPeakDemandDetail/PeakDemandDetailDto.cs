using Electricity.Application.Common.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electricity.Application.PeakDemand.Queries.GetPeakDemandDetail
{

    public class DemandSeriesDto {
        public IntervalDto TimeRange { get; set; }
        public int TimeStep { get; set; }
        public float[] ValuesMain { get; set; }
        public float[] ValuesL1 { get; set; }
        public float[] ValuesL2 { get; set; }
        public float[] ValuesL3 { get; set; }
    }

    public class PeakDemandDetailDto
    {
        public DemandSeriesDto DemandSeries1 { get; set; }
        public DemandSeriesDto DemandSeries2 { get; set; }
    }
}
