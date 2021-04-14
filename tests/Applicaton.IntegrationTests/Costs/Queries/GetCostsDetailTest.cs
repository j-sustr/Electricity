using Electricity.Application.Common.Exceptions;
using Electricity.Application.Common.Models.Dtos;
using Electricity.Application.Costs.Queries.GetCostsDetail;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Electricity.Application.IntegrationTests.Costs.Queries
{
    using static Testing;

    class GetCostsDetailTest : TestBase
    {
        [Test]
        public async Task ShouldReturnCostsOverviewWhenInfiniteIntervalProvided()
        {
            await RunAsDefaultTenantAndUser();

            var query = new GetCostsDetailQuery
            {
                GroupId = GetRecordGroupIdByName("Mistnost101"),
                Interval1 = new IntervalDto(null, null)
            };

            var result = await SendAsync(query);

            result.Items1.Should().HaveCount(GetRecordGroupCount());
            result.Items2.Should().BeNull();

            foreach (var item in result.Items1)
            {
                item.Year.Should().Be(2021);
                item.Month.Should().BeInRange(1, 12);

                item.ActiveEnergy.Should().BePositive();
                item.ReactiveEnergy.Should().BePositive();
                item.PeakDemand.Should().BePositive();
            }
        }

        [Test]
        public async Task ShouldThrowIntervalOutOfRangeException()
        {
            await RunAsDefaultTenantAndUser();

            var query = new GetCostsDetailQuery
            {
                GroupId = GetRecordGroupIdByName("Mistnost101"),
                Interval1 = new IntervalDto(new DateTime(2010, 1, 1), new DateTime(2012, 1, 1))
            };

            FluentActions.Invoking(async () => await SendAsync(query))
                .Should().Throw<IntervalOutOfRangeException>();
        }
    }
}
