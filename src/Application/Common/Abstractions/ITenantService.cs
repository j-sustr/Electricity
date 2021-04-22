using System;
using System.Collections.Generic;
using System.Text;

namespace Electricity.Application.Common.Abstractions
{
    public interface ITenantService
    {
        bool SetTenantIdentifier(string identifier);
    }
}