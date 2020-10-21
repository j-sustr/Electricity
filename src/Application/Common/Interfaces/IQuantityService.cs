using DataSource;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electricity.Application.Common.Interfaces
{
    internal interface IQuantityService
    {
        Quantity[] GetQuantities(Guid groupId, byte arch, DateRange range);
    }
}