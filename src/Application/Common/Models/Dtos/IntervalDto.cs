using Electricity.Application.Common.Mappings;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electricity.Application.Common.Models.Dtos
{
    public class IntervalDto : IMapFrom<Interval>
    {
        [JsonProperty(Required = Required.AllowNull)]
        public DateTime? Start { get; set; }

        [JsonProperty(Required = Required.AllowNull)]
        public DateTime? End { get; set; }

        public IntervalDto()
        {
        }

        public IntervalDto(DateTime? start, DateTime? end)
        {
            Start = start;
            End = end;
        }
    }
}