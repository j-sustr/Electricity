using Electricity.Application.Common.Models.Dtos;
using Electricity.Application.PeakDemand.Queries.GetPeakDemandOverview;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;

namespace Electricity.Application.IntegrationTests.PeakDemand.Queries
{
    using static Testing;

    internal class GetPeakDemandOverviewTest
    {
        [Test]
        public async Task ShouldRequireInterval1()
        {
            var userId = await RunAsDefaultUserAsync();

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
            var userId = await RunAsDefaultUserAsync();

            var query = new GetPeakDemandOverviewQuery
            {
                Interval1 = new IntervalDto(null, null)
            };

            var result = await SendAsync(query);

            result.Items1.Should().HaveCount(GetUserGroupCount());

            foreach (var item in result.Items1)
            {
                item.GroupId.Should().NotBeNullOrWhiteSpace();
                item.GroupName.Should().NotBeNullOrWhiteSpace();

                item.PeakDemandValue.Should().BePositive();
            }
        }

        [Test]
        public async Task ShouldReturnPeakDemandOverviewWhenInterval1Provided()
        {
            var userId = await RunAsDefaultUserAsync();

            var query = new GetPeakDemandOverviewQuery
            {
                Interval1 = new IntervalDto(new DateTime(2021, 1, 1), new DateTime(2021, 1, 10))
            };

            var result = await SendAsync(query);

            result.Items1.Should().HaveCount(GetUserGroupCount());

            foreach (var item in result.Items1)
            {
                item.GroupId.Should().NotBeNullOrWhiteSpace();
                item.GroupName.Should().NotBeNullOrWhiteSpace();

                item.PeakDemandTime.Should().BeAfter(query.Interval1.Start ?? DateTime.MinValue);
                item.PeakDemandTime.Should().BeBefore(query.Interval1.End ?? DateTime.MaxValue);

                item.PeakDemandValue.Should().BePositive();
            }
        }

        [Test]
        public async Task ShouldReturnPeakDemandOverviewWhen2IntervalsProvided()
        {
            var userId = await RunAsDefaultUserAsync();

            var query = new GetPeakDemandOverviewQuery
            {
                Interval1 = new IntervalDto(new DateTime(2021, 1, 1), new DateTime(2021, 1, 10)),
                Interval2 = new IntervalDto(new DateTime(2021, 1, 10), new DateTime(2021, 1, 20))
            };

            var result = await SendAsync(query);

            result.Items1.Should().HaveCount(GetUserGroupCount());
            result.Items2.Should().HaveCount(GetUserGroupCount());

            foreach (var item in result.Items1)
            {
                item.GroupId.Should().NotBeNullOrWhiteSpace();
                item.GroupName.Should().NotBeNullOrWhiteSpace();

                item.PeakDemandTime.Should().BeAfter(query.Interval1.Start ?? DateTime.MinValue);
                item.PeakDemandTime.Should().BeBefore(query.Interval1.End ?? DateTime.MaxValue);

                item.PeakDemandValue.Should().BePositive();
            }
        }
    }
}