using KMB.DataSource;
using Electricity.Application.Common.Enums;
using Electricity.Application.Common.Interfaces;
using Electricity.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Electricity.Application.Common.Services
{
    public class PowerService
    {
        private readonly IArchiveRepository _tableCollection;

        public PowerService(IArchiveRepository tableCollection)
        {
            _tableCollection = tableCollection;
        }

        public PowerRowsView GetRowsView(Guid groupId, Interval interval, PowerQuantity[] quantities)
        {
            var table = _tableCollection.GetTable(groupId, (byte)Arch.Main);

            var q = quantities.Select(q => q.ToQuantity()).ToArray();

            var rows = table.GetRows(new Models.Queries.GetRowsQuery
            {
                Interval = interval,
                Quantities = q
            });

            if (rows.Count() == 0)
            {
                return null;
            }

            return new PowerRowsView(quantities, rows);
        }

        public bool HasInterval(Guid groupId, Interval interval)
        {
            return GetIntervalOverlap(groupId, interval).Equals(interval);
        }

        public Interval GetIntervalOverlap(Guid groupId, Interval interval)
        {
            return _tableCollection
                .GetInterval(groupId, (byte)Arch.Main)
                .GetOverlap(interval);
        }
    }
}