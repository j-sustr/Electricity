using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Series
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

        IEnumerable<T> Times();

        IEnumerable<V> Values();

        IEnumerable<Tuple<T, V>> Entries();
    }

    public interface ITimeSeries<V> : ITimeSeries<DateTime, V> { }
}