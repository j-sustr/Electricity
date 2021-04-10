using System.Threading.Tasks;
using AutoMapper;
using Electricity.Application.Archive.Queries.GetQuantitySeries;
using Electricity.Application.Common.Models.Dtos;
using Electricity.WebUI.Models;
using Microsoft.AspNetCore.Mvc;

namespace Electricity.WebUI.Controllers
{
    public class SeriesController : ApiController
    {
        private readonly IMapper _mapper;

        public SeriesController(IMapper mapper)
        {
            this._mapper = mapper;
        }

        [HttpGet("quantity")]
        public async Task<ActionResult<TimeSeriesDto<float>>> GetQuantity([FromQuery] GetQuantitySeriesApiModel req)
        {
            var query = _mapper.Map<GetQuantitySeriesQuery>(req);

            var series = await Mediator.Send(query);

            return Ok(series);
        }
    }
}