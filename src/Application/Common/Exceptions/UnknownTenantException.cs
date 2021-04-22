using System;
using System.Collections.Generic;
using System.Text;

namespace Electricity.Application.Common.Exceptions
{
    public class UnknownTenantException : Exception
    {
        public UnknownTenantException()
            : base()
        {
        }
    }
}