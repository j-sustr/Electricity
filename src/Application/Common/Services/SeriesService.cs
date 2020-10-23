using Electricity.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electricity.Application.Common.Services
{
    public class SeriesService
    {
        private readonly IRowCollectionReader _reader;

        public SeriesService(IRowCollectionReader reader)
        {
            _reader = reader;
        }
    }
}