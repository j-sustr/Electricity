using AutoMapper;
using Common.Series;
using Electricity.Application.Common.Mappings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Electricity.Application.Common.Models.Dtos
{
    public class TimeSeriesDto<T>
    {
        // private IList<object[]> Entries { get; set; }
        public IList<Tuple<DateTime, T>> Entries { get; set; }

        public static TimeSeriesDto<T> FromTimeSeries(ITimeSeries<T> series)
        {
            var entries = series.Entries().ToList();

            return new TimeSeriesDto<T>
            {
                Entries = entries
            };
        }
    }
}