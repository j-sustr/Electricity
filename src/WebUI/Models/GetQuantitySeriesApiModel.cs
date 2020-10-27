using System;
using Electricity.Application.Common.Enums;

namespace Electricity.WebUI.Models
{
    public class GetQuantitySeriesApiModel
    {
        public Guid GroupId { get; set; }
        public byte Arch { get; set; }
        public string Qty { get; set; }
        public DateTime RStart { get; set; }
        public DateTime REnd { get; set; }
        public AggregationMethod AggMeth { get; set; }
        public int AggInt { get; set; }
    }
}