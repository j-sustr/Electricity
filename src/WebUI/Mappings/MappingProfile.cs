using AutoMapper;
using KMB.DataSource;
using Electricity.Application.Common.Extensions;
using Electricity.Application.Common.Models;
using Electricity.Application.Series.Queries.GetQuantitySeries;
using Electricity.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Electricity.WebUI.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<string, Quantity>()
                .ConvertUsing(s => QuantityExtensions.FromString(s));

            CreateMap<GetQuantitySeriesApiModel, GetQuantitySeriesQuery>()
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Qty));
        }
    }
}