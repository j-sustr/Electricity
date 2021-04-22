using System;
using System.Collections.Generic;
using KMB.DataSource;

namespace Electricity.Infrastructure.DataSource.Fake
{
    public class FakeRowCollection : RowCollection
    {
        private IEnumerable<RowInfo> _rows;

        public FakeRowCollection()
        {
            _rows = new RowInfo[] { };
        }

        public FakeRowCollection(IEnumerable<RowInfo> rows)
        {
            _rows = rows;
        }

        public override uint Count => throw new System.NotImplementedException();

        public override bool IsLast => throw new System.NotImplementedException();

        public override byte[] Buffer => new byte[1];

        public override IEnumerator<RowInfo> GetEnumerator()
        {
            return _rows.GetEnumerator();
        }

        public override unsafe void SetBufferAndPointer(byte[] Buffer, byte* BufferP)
        {
            throw new NotImplementedException();
        }

        public override unsafe void SetPointer(byte* BufferP)
        {
        }
    }
}