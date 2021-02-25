using AutoMapper;
using Electricity.Application.Common.Mappings;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electricity.Application.Common.Models.Dtos
{
    public class IntervalDto : IMapFrom<Interval>
    {
        public DateTime? Start { get; set; }

        public DateTime? End { get; set; }

        public bool? IsInfinite { get; set; }

        public IntervalDto()
        {
        }

        public IntervalDto(DateTime? start, DateTime? end)
        {
            Start = start;
            End = end;
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<IntervalDto, Interval>()
                .ForMember(d => d.Start, opt =>
                    opt.PreCondition(src => src.IsInfinite != true)
                )
                .ForMember(d => d.End, opt =>
                    opt.PreCondition(src => src.IsInfinite != true)
                );
        }
    }
}