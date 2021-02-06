using Electricity.Application.PowerFactor.Queries.GetPowerFactorOverviewQuery;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Electricity.WebUI.Controllers
{
    public class PowerFactorController : ApiController
    {
        [HttpGet("overview")]
        public async Task<ActionResult<PowerFactorOverviewDto>> GetOverviewAsync([FromQuery] GetPowerFactorOverviewQuery query)
        {
            return await Mediator.Send(query);
        }
    }
}