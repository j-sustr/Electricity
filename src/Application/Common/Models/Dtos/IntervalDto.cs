using Electricity.Application.Common.Mappings;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electricity.Application.Common.Models.Dtos
{
    public class IntervalDto : IMapFrom<Interval>
    {
        public DateTime? Start { get; set; }
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