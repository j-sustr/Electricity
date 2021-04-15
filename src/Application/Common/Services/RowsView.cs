using Electricity.Application.Common.Models;
using Electricity.Application.Common.Models.TimeSeries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Electricity.Application.Common.Services
{
    public abstract class RowsView<TQuantity> where TQuantity : IEquatable<TQuantity>
    {
        protected readonly TQuantity[] _quantities;

        protected readonly IEnumerable<Tuple<DateTime, float[]>> _rows;

        public RowsView(
            TQuantity[] quantities,
            IEnumerable<Tuple<DateTime, float[]>> rows
            )
        {
            _quantities = quantities;
            _rows = rows;
        }

        public DateTime GetStart()
        {
            return _rows.First().Item1;
        }

        public Interval GetInterval()
        {
            var start = _rows.First().Item1;
            var end = _rows.Last().Item1;
            return new Interval(start, end);
        }

        public Tuple<DateTime, float>[] GetSeries(TQuantity quantity)
        {
            // var series = new VariableIntervalTimeSeries<float[]>(_rows);
            int idx = GetIndexOfQuantity(quantity);
            return _rows.Select(r => Tuple.Create(r.Item1, r.Item2[idx])).ToArray();
        }

        public bool HasQuantity(TQuantity quantity)
        {
            return _quantities.Contains(quantity);
        }

        protected int GetIndexOfQuantity(TQuantity quantity)
        {
            int a = -1;
            for (int i = 0; i < _quantities.Length; i++)
            {
                if (_quantities[i].Equals(quantity))
                {
                    a = i;
                    break;
                }
            }

            if (a == -1)
            {
                throw new Exception($"does not have {nameof(quantity)}");
            }

            return a;
        }
    }
}