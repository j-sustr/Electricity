using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Series
{
    public class VariableIntervalTimeSeries<TValue> : ITimeSeries<TValue>
    {
        private readonly IEnumerable<Tuple<DateTime, TValue>> _entries;

        public int Size { get; }

        public DateTime EndTime => throw new NotImplementedException();

        public DateTime StartTime => throw new NotImplementedException();

        public VariableIntervalTimeSeries(IEnumerable<Tuple<DateTime, TValue>> entries)
        {
            Size = entries.Count();
            _entries = entries;
        }

        public TValue ValueAt(int index)
        {
            throw new NotImplementedException();
        }

        public DateTime TimeAt(int index)
        {
            throw new NotImplementedException();
        }

        public Tuple<DateTime, TValue> EntryAt(int index)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Tuple<DateTime, TValue>> Entries()
        {
            return _entries;
        }

        public IEnumerable<DateTime> Times()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TValue> Values()
        {
            throw new NotImplementedException();
        }
    }
}