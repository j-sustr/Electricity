using System;
using System.Threading.Tasks;
using Electricity.Application.PowerFactor.Queries.GetPowerFactorOverviewQuery;
using NUnit.Framework;
using FluentAssertions;
using System.Linq;
using System.Collections.Generic;

namespace Electricity.Application.IntegrationTests.PowerFactor.Queries
{
    using static Testing;

    public class GetPowerFactorOverviewTest
    {
        [Test]
        public async Task ShouldReturnEmptyResult()
        {
            var userId = await RunAsDefaultUserAsync();

            var query = new GetPowerFactorOverviewQuery();

            var result = await SendAsync(query);

            result.Interval1Items.Should().BeNull();
            result.Interval2Items.Should().BeNull();
        }

        [Test]
        public async Task ShouldReturnPowerFactorOverview()
        {
            var userId = await RunAsDefaultUserAsync();

            var query = new GetPowerFactorOverviewQuery
            {
                Interval1 = Tuple.Create(new DateTime(2021, 1, 1), new DateTime(2021, 1, 10))
            };

            var result = await SendAsync(query);

            result.Interval1Items.Should().HaveCount(3);

            foreach (var group in result.Interval1Items)
            {
                group.DeviceName.Should().NotBeNullOrWhiteSpace();
                group.Interval.Should().Be(0);
                group.ActiveEnergy.Should().BePositive();
                group.ReactiveEnergyL.Should().BePositive();
                group.ReactiveEnergyC.Should().BePositive();
                group.TanFi.Should().BePositive();
            }
        }

        [Test]
        public async Task ShouldReturnPowerFactorOverviewComparison()
        {
            var userId = await RunAsDefaultUserAsync();

            var query = new GetPowerFactorOverviewQuery
            {
                Interval1 = Tuple.Create(new DateTime(2021, 1, 1), new DateTime(2021, 1, 10)),
                Interval2 = Tuple.Create(new DateTime(2021, 1, 10), new DateTime(2021, 1, 20))
            };

            var result = await SendAsync(query);

            result.Interval1Items.Should().HaveCount(3);
            result.Interval2Items.Should().HaveCount(3);

            CheckIntervalItems(result.Interval1Items);
            CheckIntervalItems(result.Interval1Items);

            void CheckIntervalItems(IList<PowerFactorOverviewItemDto> items)
            {
                foreach (var group in items)
                {
                    group.DeviceName.Should().NotBeNullOrWhiteSpace();
                    group.Interval.Should().Be(0);
                    group.ActiveEnergy.Should().BePositive();
                    group.ReactiveEnergyL.Should().BePositive();
                    group.ReactiveEnergyC.Should().BePositive();
                    group.TanFi.Should().BePositive();
                }
            }
        }
    }
}