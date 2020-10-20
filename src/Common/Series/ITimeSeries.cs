using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Series
{
    public interface ITimeSeries<T, V>
    {
        int Size { get; }

        bool HasFixedInteval { get; }
        TimeSpan Interval { get; }

        T EndTime { get; }
        T StartTime { get; }

        T TimeAt(int index);

        V ValueAt(int index);

        Tuple<T, V> EntryAt(int index);

        IEnumerable<T> GetTimes();

        IEnumerable<V> GetValues();

        IEnumerable<Tuple<T, V>> GetEntries();
    }

    public interface ITimeSeries<V> : ITimeSeries<DateTime, V> { }
}