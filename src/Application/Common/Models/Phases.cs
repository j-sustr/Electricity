using Electricity.Application.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electricity.Application.Common.Models
{
    public class Phases
    {
        public bool? Main { get; set; }
        public bool? L1 { get; set; }
        public bool? L2 { get; set; }
        public bool? L3 { get; set; }

        public Phase[] ToArray()
        {
            var items = new List<Phase>();

            if (Main == true) items.Add(Phase.Main);
            if (L1 == true) items.Add(Phase.L1);
            if (L2 == true) items.Add(Phase.L2);
            if (L3 == true) items.Add(Phase.L3);

            return items.ToArray();
        }
    }
}