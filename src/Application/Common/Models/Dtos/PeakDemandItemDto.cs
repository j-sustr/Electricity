using Electricity.Application.Common.Mappings;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electricity.Application.Common.Models.Dtos
{
    public class PeakDemandItemDto : IMapFrom<PeakDemandItem>
    {
        public DateTime Start { get; set; }
        public float Value { get; set; }
    }
}
