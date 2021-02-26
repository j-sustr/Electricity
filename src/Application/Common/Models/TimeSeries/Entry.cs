using System;
using System.Collections.Generic;
using System.Text;

namespace Electricity.Application.Common.Models.TimeSeries
{
    public struct Entry<TTime, TValue>
    {
        public TTime Time { get; set; }
        public TValue Value { get; set; }

        public Entry(TTime time, TValue value)
        {
            Time = time;
            Value = value;
        }
    }
}