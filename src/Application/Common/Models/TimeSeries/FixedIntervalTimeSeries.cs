using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Electricity.Application.Common.Models.TimeSeries
{
    public class FixedIntervalTimeSeries<TValue> : ITimeSeries<TValue>
    {
        private readonly TValue[] _values;

        public TimeSpan Interval { get; }

        public DateTime EndTime => this.TimeAt(this.Size + 1);

        public DateTime StartTime { get; }

        public int Size { get; }

        public FixedIntervalTimeSeries(TValue[] values, DateTime startTime, TimeSpan interval)
        {
            Size = values.Length;
            _values = values;
            StartTime = startTime;
            Interval = interval;
        }

        public IEnumerable<Tuple<DateTime, TValue>> Entries()
        {
            return _values.Select((v, i) => Tuple.Create(this.TimeAt(i), v));
        }

        public Tuple<DateTime, TValue> EntryAt(int index)
        {
            var value = this._values[index];
            var time = this.TimeAt(index);
            return Tuple.Create(time, value);
        }

        public DateTime TimeAt(int index)
        {
            return this.StartTime.AddTicks(index * this.Interval.Ticks);
        }

        public IEnumerable<DateTime> Times()
        {
            throw new NotImplementedException();
        }

        public TValue ValueAt(int index)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TValue> Values()
        {
            return _values;
        }
    }
}