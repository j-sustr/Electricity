using Electricity.Application.Costs.Queries.GetCostsDetail;
using Electricity.Application.Costs.Queries.GetCostsOverview;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Electricity.WebUI.Controllers
{
    public class CostsController : ApiController
    {
        [HttpGet("overview")]
        public async Task<ActionResult<CostsOverviewDto>> GetOverviewAsync([FromQuery] GetCostsOverviewQuery query)
        {
            return await Mediator.Send(query);
        }

        [HttpGet("detail")]
        public async Task<ActionResult<CostsDetailDto>> GetDetailAsync([FromQuery] GetCostsDetailQuery query)
        {
            return await Mediator.Send(query);
        }
    }
}