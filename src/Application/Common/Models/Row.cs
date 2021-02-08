using System;
using System.Collections.Generic;
using System.Text;

namespace Electricity.Application.Common.Models
{
    public struct Row
    {
        public DateTime Time { get; set; }

        public float[] Values { get; set; }
    }
}