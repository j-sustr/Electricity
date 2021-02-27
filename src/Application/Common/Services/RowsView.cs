using Electricity.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Electricity.Application.Common.Services
{
    public abstract class RowsView<TQuantity>
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

        public Interval GetInterval()
        {
            var start = _rows.First().Item1;
            var end = _rows.Last().Item1;
            return new Interval(start, end);
        }

        public bool HasQuantity(TQuantity quantity)
        {
            return _quantities.Contains(quantity);
        }

        protected int GetIndexOfQuantity(TQuantity quantity)
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