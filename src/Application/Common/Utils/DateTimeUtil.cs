using System;
using System.Collections.Generic;
using System.Linq;
using Electricity.Application.Common.Extensions;

namespace Electricity.Application.Common.Utils
{
    public static class DateTimeUtil
    {
        public static DateTime Earliest(params DateTime[] dateTimes)
        {
            return dateTimes.Min();
        }

        public static DateTime Latest(params DateTime[] dateTimes)
        {
            return dateTimes.Max();
        }
    }
}