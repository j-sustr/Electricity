using System;
using System.Collections.Generic;
using System.Text;

namespace Electricity.Application.Common.Utils
{
    public static class ElectricityUtil
    {
        public static float CalcCosFi(float ep, float eq)
        {
            return ep / (float)Math.Sqrt(ep * ep + eq * eq);
        }
    }
}