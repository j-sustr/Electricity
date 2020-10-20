using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Series
{
    internal class VariableIntervalTimeSeries<V> : ITimeSeries<V>
    {
        private Tuple<DateTime, V> _entries;

        public int Size => throw new NotImplementedException();

        public bool HasFixedInteval => throw new NotImplementedException();

        public TimeSpan Interval => throw new NotImplementedException();

        public DateTime EndTime => throw new NotImplementedException();

        public DateTime StartTime => throw new NotImplementedException();

        public VariableIntervalTimeSeries(Tuple<DateTime, V> entries)
        {
            _entries = entries;
        }

        public IEnumerable<Tuple<DateTime, V>> GetEntries()
        {
            throw new NotImplementedException();
        }

        public Tuple<DateTime, V> EntryAt(int index)
        {
            throw new NotImplementedException();
        }

        public DateTime TimeAt(int index)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DateTime> GetTimes()
        {
            throw new NotImplementedException();
        }

        public V ValueAt(int index)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<V> GetValues()
        {
            throw new NotImplementedException();
        }
    }
}