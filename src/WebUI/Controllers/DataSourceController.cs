using AutoMapper;
using Electricity.Application.Common.Models.Dtos;
using Electricity.Application.Costs.Queries.GetCostsOverview;
using Electricity.Application.DataSource.Commands.OpenDataSource;
using Electricity.Application.DataSource.Queries.GetDataSourceInfo;
using Electricity.Infrastructure.DataSource;
using KMB.DataSource;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Electricity.WebUI.Controllers
{
    public class DataSourceController : ApiController
    {
        private ApplicationDataSource _appDS;
        private IMapper _mapper;

        public DataSourceController(
            ApplicationDataSource appDS,
            IMapper mapper)
        {
            _appDS = appDS;
            _mapper = mapper;
        }

        // --- DEBUG ---
        [HttpGet("tenant")]
        public ActionResult GetTenant()
        {
            var tenant = HttpContext.Session.GetString("__tenant__");
            return Ok(tenant);
        }

        [HttpPost("tenant")]
        public ActionResult SetTenant(string identifier)
        {
            HttpContext.Session.SetString("__tenant__", identifier);
            return Ok();
        }

        [HttpGet("records")]
        public ActionResult<List<SmpMeasNameDBDto>> GetRecords()
        {
            var records = _appDS.GetRecords();

            var dto = _mapper.Map<List<SmpMeasNameDBDto>>(records);

            return dto;
        }

        // ------

        [HttpPost("open")]
        public async Task<ActionResult> OpenAsync([FromBody] OpenDataSourceCommand query)
        {
            await Mediator.Send(query);

            return Ok();
        }

        [HttpGet("info")]
        public async Task<ActionResult<DataSourceInfoDto>> GetInfoAsync([FromQuery] GetDataSourceInfoQuery query)
        {
            return await Mediator.Send(query);
        }
    }
}