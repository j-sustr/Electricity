using System;
using System.Threading.Tasks;
using Electricity.Application.Quantities.Queries.GetQuantities;
using Microsoft.AspNetCore.Mvc;

namespace Electricity.WebUI.Controllers
{
    public class QuantitiesController : ApiController
    {
        [HttpGet("{groupId}")]
        public async Task<ActionResult<QuantitiesDto>> GetQuantities(Guid groupId, [FromQuery] byte arch)
        {
            return await Mediator.Send(new GetQuantitiesQuery
            {
                GroupId = groupId,
                Arch = arch
            });
        }
    }
}