using System;
using System.Collections.Generic;
using System.Text;

namespace Electricity.Application.Common.Enums
{
    public enum DemandAggregation
    {
        None = 1,
        OneHour = 4,
        SixHours = 24,
        TwelveHours = 48,
        OneDay = 96,
        OneWeek = 672,
    }
}
