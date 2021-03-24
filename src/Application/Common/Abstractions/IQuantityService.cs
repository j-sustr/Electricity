using KMB.DataSource;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electricity.Application.Common.Interfaces
{
    public interface IQuantityService
    {
        Quantity[] GetQuantities(Guid groupId, byte arch, DateRange range);
    }
}