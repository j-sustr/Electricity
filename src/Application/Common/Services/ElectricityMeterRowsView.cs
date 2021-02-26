using DataSource;
using Electricity.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Electricity.Application.Common.Services
{
    public class ElectricityMeterRowsView
    {
        private readonly ElectricityMeterQuantity[] _quantities;
        private IEnumerable<Tuple<DateTime, float[]>> _rows;

        public ElectricityMeterRowsView(
            ElectricityMeterQuantity[] quantities,
            IEnumerable<Tuple<DateTime, float[]>> rows)
        {
            _quantities = quantities;
            _rows = rows;
        }

        public float GetDifference(ElectricityMeterQuantity quantity)
        {
            var i = GetIndexOfQuantity(quantity);
            var firstRow = _rows.First();
            var lastRow = _rows.Last();

            return lastRow.Item2[i] - firstRow.Item2[i];
        }

        public float GetDifferencePerMonths(ElectricityMeterQuantity quantity)
        {
            throw new Exception("not implemented");
        }

        public Interval GetInterval()
        {
            var start = _rows.First().Item1;
            var end = _rows.Last().Item1;
            return new Interval(start, end);
        }

        public bool HasQuantity(ElectricityMeterQuantity quantity)
        {
            return _quantities.Contains(quantity);
        }

        private int GetIndexOfQuantity(ElectricityMeterQuantity quantity)
        {
            int i = Array.IndexOf(_quantities, quantity);
            if (i == -1)
            {
                throw new Exception($"does not have {nameof(quantity)}");
            }

            return i;
        }
    }
}