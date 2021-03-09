using Electricity.Application.Common.Exceptions;
using Electricity.Application.Common.Models.Dtos;
using Electricity.Application.Costs.Queries.GetCostsOverview;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Electricity.Application.IntegrationTests.Costs.Queries
{
    using static Testing;

    public class GetCostsOverviewTest
    {
        [Test]
        public async Task ShouldReturnCostsOverviewWhenInfiniteIntervalProvided()
        {
            var userId = await RunAsDefaultUserAsync();

            var query = new GetCostsOverviewQuery
            {
                Interval1 = new IntervalDto(null, null)
            };

            var result = await SendAsync(query);

            result.Items1.Should().HaveCount(GetGroupCount());
            result.Items2.Should().BeNull();

            foreach (var item in result.Items1)
            {
                item.GroupId.Should().NotBeNullOrWhiteSpace();
                item.GroupName.Should().NotBeNullOrWhiteSpace();

                item.ActiveEnergyInMonths.Should().OnlyContain(x => x > 0);
                item.ReactiveEnergyInMonths.Should().OnlyContain(x => x > 0);
                item.PeakDemandInMonths.Should().OnlyContain(x => x > 0);
            }
        }

        [Test]
        public async Task ShouldThrowIntervalOutOfRangeException()
        {
            var userId = await RunAsDefaultUserAsync();

            var query = new GetCostsOverviewQuery
            {
                Interval1 = new IntervalDto(new DateTime(2010, 1, 1), new DateTime(2012, 1, 1))
            };

            FluentActions.Invoking(async () => await SendAsync(query))
                .Should().Throw<IntervalOutOfRangeException>();
        }
    }
}