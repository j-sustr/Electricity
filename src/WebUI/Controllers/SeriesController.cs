using System;
using System.ComponentModel;
using System.Threading.Tasks;
using AutoMapper;
using Common.Series;
using DataSource;
using Electricity.Application.Series.Queries.GetQuantitySeries;
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

        [HttpGet("hello")]
        public async Task<ActionResult> GetHello()
        {
            var hello = await Task.FromResult("hello");
            return Ok(hello);
        }

        [HttpGet("quantity")]
        public async Task<ActionResult<ITimeSeries<float>>> GetQuantity([FromQuery] GetQuantitySeriesApiModel req)
        {
            var query = _mapper.Map<GetQuantitySeriesQuery>(req);

            var series = await Mediator.Send(query);

            return Ok(series);
        }
    }
}