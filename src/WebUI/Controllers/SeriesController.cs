using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Common.Series;
using DataSource;
using Electricity.Application.Series.Queries.GetQuantitySeries;
using Electricity.WebUI.Models;
using Microsoft.AspNetCore.Mvc;

namespace Electricity.WebUI.Controllers
{
    public class SeriesController : ApiController
    {
        [HttpGet("hello")]
        public async Task<ActionResult> GetHello()
        {
            var hello = await Task.FromResult("hello");
            return Ok(hello);
        }

        [HttpGet("quantity/{groupId}")]
        public async Task<ActionResult<ITimeSeries<float>>> GetQuantity([FromQuery] GetQuantitySeriesApiModel req)
        {
            var quantityConverter = TypeDescriptor.GetConverter(typeof(Quantity));

            var query = new GetQuantitySeriesQuery
            {
                GroupId = req.GroupId,
                Arch = req.Arch,
                Quantity = (Quantity)quantityConverter.ConvertFromString(req.Qty),

            };

            var series = await Mediator.Send(query);

            return Ok(series);
        }

    }
}