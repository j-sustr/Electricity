﻿using KMB.DataSource;
using System.Linq;

namespace Electricity.Infrastructure.DataSource.Util
{
    public static class GroupInfoUtil
    {
        public static GroupInfo CloneGroupInfo(GroupInfo g)
        {
            return new GroupInfo
            {
                ID = g.ID,
                Name = g.Name,
                Archives = g.Archives?.Select(a =>
                {
                    if (a == null) return null;
                    var newA = new ArchiveInfo(a.Arch);
                    newA.Count = a.Count;
                    newA.Range = a.Range != null ? new DateRange(a.Range.DateMin, a.Range.DateMax) : null;
                    newA.Intervals = a.Intervals?.Select(i => new DateRange(i.DateMin, i.DateMax)).ToList();
                    return newA;
                }).ToArray(),
                Subgroups = g.Subgroups.Select(g => CloneGroupInfo(g)).ToList()
            };
        }
    }
}
