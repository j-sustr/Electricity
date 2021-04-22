using AutoMapper;
using KMB.DataSource;
using Electricity.Application.Common.Models.Dtos;

namespace Electricity.WebUI.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<SmpMeasNameDB, SmpMeasNameDBDto>();
        }
    }
}