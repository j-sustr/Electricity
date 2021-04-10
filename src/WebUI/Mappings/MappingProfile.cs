using AutoMapper;
using KMB.DataSource;
using Electricity.Application.Common.Extensions;
using Electricity.WebUI.Models;
using Electricity.Application.Common.Models.Dtos;
using Electricity.Application.Archive.Queries.GetQuantitySeries;

namespace Electricity.WebUI.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<SmpMeasNameDB, SmpMeasNameDBDto>();

            CreateMap<string, Quantity>()
                .ConvertUsing(s => QuantityExtensions.FromString(s));

            CreateMap<GetQuantitySeriesApiModel, GetQuantitySeriesQuery>()
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Qty));
        }
    }
}