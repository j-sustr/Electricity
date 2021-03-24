using KMB.DataSource;
using Electricity.Application.Common.Mappings;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electricity.Application.Quantities.Queries.GetQuantities
{
    public class QuantityDto : IMapFrom<Quantity>
    {
        public string PropName { get; set; }
        public string Unit { get; set; }
        public string ReturnType { get; set; }
        public string Prop { get; set; }
        public object Value { get; set; }
    }
}