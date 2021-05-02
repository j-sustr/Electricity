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

    public class GetCostsOverviewTest : TestBase
    {
        [Test]
        public async Task ShouldReturnCostsOverviewWhenInfiniteIntervalProvided()
        {
            await RunAsDefaultTenantAndUser();

            var query = new GetCostsOverviewQuery
            {
                Interval1 = new IntervalDto(null, null)
            };

            var result = await SendAsync(query);

            result.Items1.Should().HaveCount(GetRecordGroupCount());
            result.Items2.Should().BeNull();

            foreach (var item in result.Items1)
            {
                item.GroupId.Should().NotBeNullOrWhiteSpace();
                item.GroupName.Should().NotBeNullOrWhiteSpace();

                item.ActiveEnergyInMonths.Should().OnlyContain(x => x > 0);
                item.ReactiveEnergyInMonths.Should().OnlyContain(x => x > 0);
                item.PeakDemandInMonths.Should().OnlyContain(x => x > 0);
                item.CosFiInMonths.Should().OnlyContain(x => x > 0);
            }
        }

        [Test]
        public async Task ShouldReturnEmptyResult()
        {
            await RunAsDefaultTenantAndUser();

            var query = new GetCostsOverviewQuery
            {
                Interval1 = new IntervalDto(new DateTime(2010, 1, 1), new DateTime(2012, 1, 1))
            };

            var result = await SendAsync(query);

            result.Should().NotBeNull();
            result.Items1.Should().HaveCount(GetRecordGroupCount());

            foreach (var item in result.Items1)
            {
                item.GroupId.Should().NotBeNullOrWhiteSpace();
                item.GroupName.Should().NotBeNullOrWhiteSpace();

                item.Message.Should().Be("Archives have no data on specified range: Main, ElectricityMeter, ");
            }
        }
    }
}