using Electricity.Application.Common.Enums;
using Electricity.Application.Common.Exceptions;
using Electricity.Application.Common.Models.Dtos;
using Electricity.Application.PeakDemand.Queries.GetPeakDemandDetail;
using FluentAssertions;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Electricity.Application.IntegrationTests.PeakDemand.Queries
{
    using static Testing;
    class GetPeakDemandDetailTest : TestBase
    {
        [Test]
        public async Task ShouldRequireGroupId()
        {
            await RunAsDefaultTenantAndUser();

            var query = new GetPeakDemandDetailQuery
            {
                Interval1 = new IntervalDto(null, null),
                Aggregation = DemandAggregation.None
            };

            FluentActions.Invoking(() =>
                SendAsync(query)).Should().Throw<ValidationException>();
        }

        [Test]
        public async Task ShouldRequireValidAggregation()
        {
            await RunAsDefaultTenantAndUser();

            var query = new GetPeakDemandDetailQuery
            {
                GroupId = GetRecordGroupIdByName("Mistnost101"),
                Interval1 = new IntervalDto(null, null),
                Aggregation = 0
            };

            FluentActions.Invoking(() =>
                SendAsync(query)).Should().Throw<ValidationException>();
        }


        [Test]
        public async Task ShouldReturnPeakDemandDetailWhenUnboundedIntervalProvided()
        {
            await RunAsDefaultTenantAndUser();

            var query = new GetPeakDemandDetailQuery
            {
                GroupId = GetRecordGroupIdByName("Mistnost101"),
                Interval1 = new IntervalDto(null, null),
                Aggregation = DemandAggregation.None,
            };

            var result = await SendAsync(query);

            result.DemandSeries1.Should().NotBeNull();
            var series1 = result.DemandSeries1;
            series1.TimeStep.Should().BePositive();
            series1.TimeRange.Should().NotBeNull();
            series1.ValuesMain.Should().NotBeEmpty();

            result.DemandSeries2.Should().BeNull();
        }

        [Test]
        public async Task ShouldReturnPeakDemandDetailWhenUnboundedIntervalAndAggregationProvided()
        {
            await RunAsDefaultTenantAndUser();

            var query = new GetPeakDemandDetailQuery
            {
                GroupId = GetRecordGroupIdByName("Mistnost101"),
                Interval1 = new IntervalDto(null, null),
                Aggregation = DemandAggregation.OneDay,
            };

            var result = await SendAsync(query);

            result.DemandSeries1.Should().NotBeNull();
            var series1 = result.DemandSeries1;
            series1.TimeStep.Should().BePositive();
            series1.TimeRange.Should().NotBeNull();
            series1.ValuesMain.Should().NotBeEmpty();

            result.DemandSeries2.Should().BeNull();
        }
    }
}
