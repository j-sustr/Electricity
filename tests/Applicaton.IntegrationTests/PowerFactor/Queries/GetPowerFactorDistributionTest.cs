using System;
using System.Threading.Tasks;
using NUnit.Framework;
using FluentAssertions;
using Electricity.Application.Common.Exceptions;
using Electricity.Application.Common.Models.Dtos;
using Electricity.Application.PowerFactor.Queries.GetPowerFactorDistribution;

namespace Electricity.Application.IntegrationTests.PowerFactor.Queries
{
    using static Testing;

    public class GetPowerFactorDistributionTest
    {
        [Test]
        public async Task ShouldRequireInterval1()
        {
            var userId = await RunAsDefaultUserAsync();

            var query = new GetPowerFactorDistributionQuery
            {
                Interval1 = null
            };

            FluentActions.Invoking(() =>
                SendAsync(query)).Should().Throw<ValidationException>();
        }

        [Test]
        public async Task ShouldReturnPowerFactorOverviewWhenUnboundedIntervalProvided()
        {
            var userId = await RunAsDefaultUserAsync();

            var query = new GetPowerFactorDistributionQuery
            {
                Interval1 = new IntervalDto(null, null)
            };

            var result = await SendAsync(query);

            result.GroupName.Should().NotBeNullOrWhiteSpace();
            result.Distribution1.Should().HaveCount(1);

            foreach (var item in result.Distribution1)
            {
                item.Value.Should().BePositive();
                item.Range.Should().NotBeNullOrWhiteSpace();
            }
        }

        [Test]
        public async Task ShouldReturnPowerFactorOverviewWhenInterval1Provided()
        {
            var userId = await RunAsDefaultUserAsync();

            var query = new GetPowerFactorDistributionQuery
            {
                Interval1 = new IntervalDto(new DateTime(2021, 1, 1), new DateTime(2021, 1, 10))
            };

            var result = await SendAsync(query);

            result.GroupName.Should().NotBeNullOrWhiteSpace();
            result.Distribution1.Should().HaveCount(7);

            foreach (var item in result.Distribution1)
            {
                item.Value.Should().BePositive();
                item.Range.Should().NotBeNullOrWhiteSpace();
            }
        }

        [Test]
        public async Task ShouldReturnPowerFactorOverviewWhen2IntervalsProvided()
        {
            var userId = await RunAsDefaultUserAsync();

            var query = new GetPowerFactorDistributionQuery
            {
                Interval1 = new IntervalDto(new DateTime(2021, 1, 1), new DateTime(2021, 1, 10)),
                Interval2 = new IntervalDto(new DateTime(2021, 1, 10), new DateTime(2021, 1, 20))
            };

            var result = await SendAsync(query);

            result.Distribution1.Should().HaveCount(2);
            result.Distribution1.Should().HaveCount(2);

            foreach (var item in result.Distribution1)
            {
                item.Value.Should().BePositive();
                item.Range.Should().NotBeNullOrWhiteSpace();
            }

            foreach (var item in result.Distribution2)
            {
                item.Value.Should().BePositive();
                item.Range.Should().NotBeNullOrWhiteSpace();
            }
        }
    }
}