using System;
using System.Threading.Tasks;
using Electricity.Application.Archive.Queries.GetQuantities;
using Electricity.Application.Archive.Queries.GetQuantitySeries;
using Microsoft.AspNetCore.Mvc;

namespace Electricity.WebUI.Controllers
{
    public class ArchiveController : ApiController
    {
        [HttpGet("quantities")]
        public async Task<ActionResult<QuantitiesDto>> GetQuantities([FromQuery] Guid groupId, [FromQuery] byte arch)
        {
            return await Mediator.Send(new GetQuantitiesQuery
            {
                GroupId = groupId,
                Arch = arch
            });
        }

        [HttpGet("series")]
        public async Task<ActionResult<QuantitySeriesDto>> GetSeries([FromQuery] GetQuantitySeriesQuery query)
        {
            return await Mediator.Send(query);
        }

    }
}