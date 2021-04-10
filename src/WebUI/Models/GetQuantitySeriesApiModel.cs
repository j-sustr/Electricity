using System;
using Electricity.Application.Archive.Queries.GetQuantitySeries;
using Electricity.Application.Common.Enums;
using Electricity.Application.Common.Mappings;

namespace Electricity.WebUI.Models
{
    public class GetQuantitySeriesApiModel : IMapFrom<GetQuantitySeriesQuery>
    {
        public Guid GroupId { get; set; }
        public byte Arch { get; set; }
        public string Qty { get; set; }

        // public DateTime? RStart { get; set; }
        // public DateTime? REnd { get; set; }
        public AggregationMethod AggMeth { get; set; }

        public int AggInt { get; set; }
    }
}