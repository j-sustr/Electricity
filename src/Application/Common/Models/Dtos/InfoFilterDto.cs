using AutoMapper;
using Electricity.Application.Common.Mappings;
using KMB.DataSource;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electricity.Application.Common.Models.Dtos
{
    public class InfoFilterDto : IMapFrom<InfoFilter>
    {
        public bool RecurseSubgroups { get; set; }
        public int[] Archs { get; set; }
        public ArchiveInfoType infoType { get; set; }
        public DateRangeDto range { get; set; }
        public bool IDisGroup { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<InfoFilterDto, InfoFilter>()
                .ReverseMap();
        }
    }
}