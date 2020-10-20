using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Common.Series
{
    public class FixedIntervalTimeSeries<V> : ITimeSeries<V>
    {
        private int _size;
        private V[] _values;
        private DateTime _startTime;
        private TimeSpan _interval;

        public bool HasFixedInteval => true;

        public TimeSpan Interval => this._interval;

        public DateTime EndTime => this.TimeAt(this._size + 1);

        public DateTime StartTime => this._startTime;

        public int Size => this._size;

        public FixedIntervalTimeSeries(V[] values, DateTime startTime, TimeSpan interval)
        {
            _size = values.Length;
            _values = values;
            _startTime = startTime;
            _interval = interval;
        }

        public IEnumerable<Tuple<DateTime, V>> GetEntries()
        {
            return this._values.Select((v, i) => Tuple.Create(this.TimeAt(i), v));
        }

        public Tuple<DateTime, V> EntryAt(int index)
        {
            var value = this._values[index];
            var time = this.TimeAt(index);
            return Tuple.Create(time, value);
        }

        public DateTime TimeAt(int index)
        {
            return this.StartTime.AddTicks(index * this._interval.Ticks);
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