using System;
using System.Collections.Generic;
using System.Text;

namespace Electricity.Application.Common.Models
{
    public class PeakDemandItem
    {
        public DateTime IntervalStart { get; set; }
        public DateTime Start { get; set; }
        public float Value { get; set; }
    }
}
