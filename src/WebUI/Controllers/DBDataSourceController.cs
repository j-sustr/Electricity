using AutoMapper;
using Electricity.Application.Common.Models.Dtos;
using Electricity.Application.Common.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Electricity.WebUI.Controllers
{
    // DEBUG
    public class DBDataSourceController : ApiController
    {
        private ApplicationDataSource _appDS;
        private IMapper _mapper;

        public DBDataSourceController(
            ApplicationDataSource appDS,
            IMapper mapper)
        {
            _appDS = appDS;
            _mapper = mapper;
        }

        [HttpGet("records")]
        public ActionResult<List<SmpMeasNameDBDto>> GetRecords()
        {
            var records = _appDS.GetRecords();

            var dto = _mapper.Map<List<SmpMeasNameDBDto>>(records);

            return dto;
        }
    }
}