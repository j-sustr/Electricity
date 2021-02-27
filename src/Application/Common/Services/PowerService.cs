using DataSource;
using Electricity.Application.Common.Enums;
using Electricity.Application.Common.Interfaces;
using Electricity.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Electricity.Application.Common.Services
{
    public enum PowerQuantity
    {
        PAvg3P,
    }

    public class PowerService
    {
        private readonly ITableCollection _tableCollection;

        public PowerService(ITableCollection tableCollection)
        {
            _tableCollection = tableCollection;
        }

        public PowerRowsView GetRowsView(Guid groupId, Interval interval, PowerQuantity[] quantities)
        {
            var table = _tableCollection.GetTable(groupId, (byte)Arch.Main);

            var q = quantities.Select(q => GetQuantity(q)).ToArray();

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

        public static Quantity GetQuantity(PowerQuantity quantity)
        {
            switch (quantity)
            {
                case PowerQuantity.PAvg3P:
                    return new Quantity("P_max_P3_C", "W");

                default:
                    throw new ArgumentException("invalid quantity");
            }
        }

        public Interval GetIntervalOverlap(Guid groupId, Interval interval)
        {
            return _tableCollection.GetIntervalOverlap(groupId, (byte)Arch.Main, interval);
        }
    }
}