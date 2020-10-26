using DataSource;
using Electricity.Application.Common.Mappings;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electricity.Application.Quantities.Queries.GetQuantities
{
    internal class QuantityDto : IMapFrom<Quantity>
    {
        public string PropName;
        public string Unit;
        public string ReturnType;
        public string Prop;
        public object Value;
    }
}