using Electricity.Application.Costs.Queries.GetCostsOverview;
using Electricity.Application.DataSource.Commands.OpenDataSource;
using Electricity.Application.DataSource.Queries.GetDataSourceInfo;
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