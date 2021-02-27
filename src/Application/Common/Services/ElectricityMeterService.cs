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
    public enum ElectricityMeterQuantity
    {
        ActiveEnergy,
        ReactiveEnergyL,
        ReactiveEnergyC,
    }

    public class ElectricityMeterService
    {
        private readonly ITableCollection _tableCollection;

        public ElectricityMeterService(
            ITableCollection tableCollection)
        {
            _tableCollection = tableCollection;
        }

        public ElectricityMeterRowsView GetRowsView(Guid groupId, Interval interval, ElectricityMeterQuantity[] quantities)
        {
            var table = _tableCollection.GetTable(groupId, (byte)Arch.ElectricityMeter);

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

            return new ElectricityMeterRowsView(quantities, rows);
        }

        public static Quantity GetQuantity(ElectricityMeterQuantity quantity)
        {
            switch (quantity)
            {
                case ElectricityMeterQuantity.ActiveEnergy:
                    return new Quantity("3EP", "Wh");

                case ElectricityMeterQuantity.ReactiveEnergyL:
                    return new Quantity("3EQL", "varh");

                case ElectricityMeterQuantity.ReactiveEnergyC:
                    return new Quantity("3EQC", "varh");

                default:
                    throw new ArgumentException("invalid quantity");
            }
        }

        public Interval GetIntervalOverlap(Guid groupId, Interval interval)
        {
            return _tableCollection.GetIntervalOverlap(groupId, (byte)Arch.ElectricityMeter, interval);
        }
    }
}