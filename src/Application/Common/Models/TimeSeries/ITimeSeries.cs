using System;
using System.Collections.Generic;
using System.Text;

namespace Electricity.Application.Common.Models.TimeSeries
{
    public interface ITimeSeries<TTime, TValue>
    {
        int Size { get; }

        TTime EndTime { get; }
        TTime StartTime { get; }

        TTime TimeAt(int index);

        TValue ValueAt(int index);

        Tuple<TTime, TValue> EntryAt(int index);

        IEnumerable<TTime> Times();

        IEnumerable<TValue> Values();

        IEnumerable<Tuple<TTime, TValue>> Entries();
    }

    public interface ITimeSeries<TValue> : ITimeSeries<DateTime, TValue> { }
}