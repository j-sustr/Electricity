using Electricity.Application.Common.Abstractions;
using Electricity.Application.DataSource.Commands.OpenDataSource;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Electricity.WebUI.Controllers
{
    public class DataSourceController : ApiController
    {
        private readonly IDataSourceCache _dsCache;

        public DataSourceController(IDataSourceCache dsCache)
        {
            _dsCache = dsCache;
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

        // ------

        [HttpPost("open")]
        public async Task<DataSourceInfoDto> OpenAsync([FromBody] OpenDataSourceCommand command)
        {
            return await Mediator.Send(command);
        }

        [HttpDelete("cache")]
        [Authorize]
        public ActionResult ClearCache()
        {
            _dsCache.Clear();

            return Ok();
        }
    }
}