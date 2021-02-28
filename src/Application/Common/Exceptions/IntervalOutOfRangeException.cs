using System;
using System.Collections.Generic;
using System.Text;

namespace Electricity.Application.Common.Exceptions
{
    public class IntervalOutOfRangeException : Exception
    {
        public IntervalOutOfRangeException()
            : base()
        {
        }

        public IntervalOutOfRangeException(string name)
            : base($"Interval \"{name}\" is out of range.")
        {
        }
    }
}