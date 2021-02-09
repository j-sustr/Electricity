using System;
using System.Collections.Generic;
using System.Text;

namespace Electricity.Application.Common.Models
{
    public class BoundedInterval
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public BoundedInterval(DateTime start, DateTime end)
        {
            Start = start;
            End = end;
        }

        public Interval ToInterval()
        {
            return new Interval(Start, End);
        }
    }
}