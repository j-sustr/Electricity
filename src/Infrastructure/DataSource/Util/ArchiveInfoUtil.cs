using KMB.DataSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Electricity.Infrastructure.DataSource.Util
{
    public static class ArchiveInfoUtil
    {
        public static ArchiveInfo CreateArchiveInfo(byte arch, int count, DateRange range, List<DateRange> intervals)
        {
            var newA = new ArchiveInfo(arch);
            newA.Count = count;
            newA.Range = range != null ? new DateRange(range.DateMin, range.DateMax) : null; 
            newA.Intervals = intervals?.Select(i => new DateRange(i.DateMin, i.DateMax)).ToList();
            return newA;
        } 
    }
}
