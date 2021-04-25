using System;
using System.Threading.Tasks;
using Electricity.Application.Archive.Queries.GetQuantities;
using Electricity.Application.Archive.Queries.GetQuantitySeries;
using Electricity.Application.Common.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

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

        [HttpGet("query-records")]
        public ActionResult<ArchiveQueryRecord[]> GetQueries([FromQuery] GetQuantitySeriesQuery query)
        {
            var archiveRepo = HttpContext.RequestServices.GetService<ArchiveRepositoryService>();

            return archiveRepo.GetQueryRecords();
        }

    }
}