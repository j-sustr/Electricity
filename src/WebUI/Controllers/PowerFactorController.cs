using Electricity.Application.Common.Models;
using Electricity.Application.Common.Services;
using Electricity.Application.PowerFactor.Queries.GetPowerFactorDistribution;
using Electricity.Application.PowerFactor.Queries.GetPowerFactorOverview;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Electricity.Application.Common.Enums;
using System.Linq;
using System.Collections.Generic;
using KMB.DataSource;
using Electricity.Application.Common.Models.Quantities;

namespace Electricity.WebUI.Controllers
{
    public class PowerFactorController : ApiController
    {
        [HttpGet("overview")]
        public async Task<ActionResult<PowerFactorOverviewDto>> GetOverviewAsync([FromQuery] GetPowerFactorOverviewQuery query)
        {
            return await Mediator.Send(query);
        }

        [HttpGet("distribution")]
        public async Task<ActionResult<PowerFactorDistributionDto>> GetDistributionAsync([FromQuery] GetPowerFactorDistributionQuery query)
        {
            return await Mediator.Send(query);
        }

        // DEBUG
        [HttpGet("energy-series")]
        public ActionResult<Tuple<string[], object[][]>> GetEnergySeries(Guid groupId, [FromQuery] Phase[] phases, uint aggregation, EEnergyAggType EnergyAggType)
        {
            var archiveRepo = HttpContext.RequestServices.GetService<ArchiveRepositoryService>();

            var quantities = PowerFactorDistribution.GetQuantities(phases);

            var rowsView = archiveRepo.GetMainRowsView(new GetMainRowsViewQuery
            {
                GroupId = groupId,
                Range = Interval.Unbounded,
                Quantities = quantities,
                Aggregation = aggregation,
                EnergyAggType = EnergyAggType
            });

            var colNames = quantities.Select(q => q.ToQuantity().ToString())
                .Prepend("time")
                .ToArray();

            object[][] rows = rowsView.GetSeriesBundle(quantities).Select((entry) =>
            {
                return entry.Item2.Cast<object>().Prepend(entry.Item1).ToArray();
            }).ToArray();

            return Tuple.Create(colNames, rows);
        }


        [HttpGet("cos-fi-series")]
        public ActionResult<float[]> GetCosFiSeries(Guid groupId, Phase phase, uint aggregation, EEnergyAggType EnergyAggType)
        {
            var archiveRepo = HttpContext.RequestServices.GetService<ArchiveRepositoryService>();

            var quantities = PowerFactorDistribution.GetQuantities(new Phase[] { phase });

            var rowsView = archiveRepo.GetMainRowsView(new GetMainRowsViewQuery
            {
                GroupId = groupId,
                Range = Interval.Unbounded,
                Quantities = quantities,
                Aggregation = aggregation,
                EnergyAggType = EnergyAggType
            });

            return rowsView.GetColumnValues(new MainQuantity
            {
                Type = MainQuantityType.CosFi,
                Phase = phase
            }).ToArray();
        }

    }
}