using AutoMapper;
using Electricity.Application.Common.Models.Dtos;
using Electricity.Infrastructure.DataSource;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Electricity.WebUI.Controllers
{
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