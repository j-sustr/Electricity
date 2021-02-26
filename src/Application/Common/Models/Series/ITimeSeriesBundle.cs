using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Series
{
    public interface ITimeSeriesBundle<TTime, TValue>
    {
        IEnumerable<string> Keys { get; }

        int Size { get; }
        TTime EndTime { get; }
        TTime StartTime { get; }

        TTime TimeAt(int index);

        TValue ValueAt(string key, int index);

        Tuple<TTime, TValue> EntryAt(string key, int index);

        IEnumerable<TTime> GetTimes();

        IEnumerable<TValue> GetValues(string key);

        IEnumerable<Tuple<TTime, TValue>> GetEntries(string key);
    }

    public interface ITimeSeriesBundle<TValue> : ITimeSeriesBundle<DateTime, TValue> { }
}