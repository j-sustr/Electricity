using Electricity.Application.PeakDemand.Queries.GetPeakDemandDetail;
using Electricity.Application.PeakDemand.Queries.GetPeakDemandOverview;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Electricity.WebUI.Controllers
{
    public class PeakDemandController : ApiController
    {
        [HttpGet("overview")]
        public async Task<ActionResult<PeakDemandOverviewDto>> GetOverviewAsync([FromQuery] GetPeakDemandOverviewQuery query)
        {
            return await Mediator.Send(query);
        }

        [HttpGet("detail")]
        public async Task<ActionResult<PeakDemandDetailDto>> GetDetailAsync([FromQuery] GetPeakDemandDetailQuery query)
        {
            return await Mediator.Send(query);
        }
    }
}
