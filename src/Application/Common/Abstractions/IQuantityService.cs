using KMB.DataSource;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electricity.Application.Common.Interfaces
{
    public interface IQuantityService
    {
        Quantity[] GetQuantities(string groupId, byte arch, DateRange range);
    }
}