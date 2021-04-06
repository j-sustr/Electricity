using AutoMapper;
using Electricity.Application.Common.Mappings;
using KMB.DataSource;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electricity.Application.Common.Models.Dtos
{
    public class DateRangeDto : IMapFrom<DateRange>
    {
        public DateTime DateMin { get; set; }
        public DateTime DateMax { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<DateRangeDto, DateRange>()
                .ReverseMap();
        }
    }
}