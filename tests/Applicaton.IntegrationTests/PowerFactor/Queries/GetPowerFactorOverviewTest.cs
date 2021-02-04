using System;
using System.Threading.Tasks;
using Electricity.Application.PowerFactor.Queries.GetPowerFactorOverviewQuery;
using NUnit.Framework;
using FluentAssertions;
using System.Linq;

namespace Electricity.Application.IntegrationTests.PowerFactor.Queries
{
    using static Testing;

    public class GetPowerFactorOverviewTest
    {
        [Test]
        public async Task ShouldReturnPowerFactorOverview()
        {
            var userId = await RunAsDefaultUserAsync();

            var query = new GetPowerFactorOverviewQuery
            {
                Interval1 = Tuple.Create(new DateTime(2021, 1, 1), new DateTime(2021, 1, 10))
            };

            var result = await SendAsync(query);

            result.Items.Should().HaveCount(10);
            var first = result.Items.First();
            first.DeviceName = "dev1";
            first.Interval = 0;
            first.ActiveEnergy = 0;
            first.ReactiveEnergyL = 0;
            first.ReactiveEnergyC = 0;
            first.CosFi = 0;
        }
    }
}