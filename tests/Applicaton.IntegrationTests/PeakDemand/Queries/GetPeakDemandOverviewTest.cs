using Electricity.Application.Common.Exceptions;
using Electricity.Application.Common.Models.Dtos;
using Electricity.Application.PeakDemand.Queries.GetPeakDemandOverview;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Electricity.Application.IntegrationTests.PeakDemand.Queries
{
    using static Testing;

    internal class GetPeakDemandOverviewTest : TestBase
    {
        [Test]
        public async Task ShouldRequireInterval1()
        {
            await RunAsDefaultTenantAndUser();

            var query = new GetPeakDemandOverviewQuery
            {
                Interval1 = null
            };

            FluentActions.Invoking(() =>
                SendAsync(query)).Should().Throw<ValidationException>();
        }

        [Test]
        public async Task ShouldReturnPeakDemandOverviewWhenUnboundedIntervalProvided()
        {
            await RunAsDefaultTenantAndUser();

            var query = new GetPeakDemandOverviewQuery
            {
                Interval1 = new IntervalDto(null, null)
            };

            var result = await SendAsync(query);

            result.Items1.Should().HaveCount(GetRecordGroupCount());

            foreach (var item in result.Items1)
            {
                item.GroupId.Should().NotBeNullOrWhiteSpace();
                item.GroupName.Should().NotBeNullOrWhiteSpace();

                item.PeakDemands.Should().NotBeEmpty();

                foreach (var pd in item.PeakDemands)
                {
                    pd.Start.Should().BeOnOrAfter(query.Interval1.Start ?? DateTime.MinValue);
                    pd.Start.Should().BeOnOrBefore(query.Interval1.End ?? DateTime.MaxValue);

                    pd.Value.Should().BePositive();
                }
            }
        }

        [Test]
        public async Task ShouldReturnPeakDemandOverviewWhenInterval1Provided()
        {
            await RunAsDefaultTenantAndUser();

            var query = new GetPeakDemandOverviewQuery
            {
                Interval1 = new IntervalDto(new DateTime(2021, 1, 1), new DateTime(2021, 1, 10))
            };

            var result = await SendAsync(query);

            result.Items1.Should().HaveCount(GetRecordGroupCount());

            foreach (var item in result.Items1)
            {
                item.GroupId.Should().NotBeNullOrWhiteSpace();
                item.GroupName.Should().NotBeNullOrWhiteSpace();

                foreach (var pd in item.PeakDemands)
                {
                    pd.Start.Should().BeOnOrAfter(query.Interval1.Start ?? DateTime.MinValue);
                    pd.Start.Should().BeOnOrBefore(query.Interval1.End ?? DateTime.MaxValue);

                    pd.Value.Should().BePositive();
                }
            }
        }

        [Test]
        public async Task ShouldReturnPeakDemandOverviewWhen2IntervalsProvided()
        {
            await RunAsDefaultTenantAndUser();

            var query = new GetPeakDemandOverviewQuery
            {
                Interval1 = new IntervalDto(new DateTime(2021, 1, 1), new DateTime(2021, 1, 10)),
                Interval2 = new IntervalDto(new DateTime(2021, 1, 10), new DateTime(2021, 1, 20))
            };

            var result = await SendAsync(query);

            result.Items1.Should().HaveCount(GetRecordGroupCount());
            result.Items2.Should().HaveCount(GetRecordGroupCount());

            foreach (var item in result.Items1)
            {
                item.GroupId.Should().NotBeNullOrWhiteSpace();
                item.GroupName.Should().NotBeNullOrWhiteSpace();

                foreach (var pd in item.PeakDemands)
                {
                    pd.Start.Should().BeOnOrAfter(query.Interval1.Start ?? DateTime.MinValue);
                    pd.Start.Should().BeOnOrBefore(query.Interval1.End ?? DateTime.MaxValue);

                    pd.Value.Should().BePositive();
                }
            }
        }
    }
}