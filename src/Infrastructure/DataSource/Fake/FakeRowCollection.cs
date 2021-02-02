using System;
using System.Collections.Generic;
using DataSource;
using DS = DataSource;

namespace Electricity.Infrastructure.DataSource.Fake
{
    public class FakeRowCollection : DS.RowCollection
    {
        private IEnumerable<RowInfo> _rows;

        public FakeRowCollection(IEnumerable<RowInfo> rows)
        {
            _rows = rows;
        }

        public override uint Count => throw new System.NotImplementedException();

        public override bool IsLast => throw new System.NotImplementedException();

        public override byte[] Buffer => throw new System.NotImplementedException();

        public override IEnumerator<RowInfo> GetEnumerator()
        {
            return _rows.GetEnumerator();
        }

        public override unsafe void SetPointer(byte* BufferP)
        {
            throw new System.NotImplementedException();
        }
    }
}