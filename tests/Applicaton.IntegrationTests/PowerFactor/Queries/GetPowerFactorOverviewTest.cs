using System;
using System.Threading.Tasks;
using NUnit.Framework;
using FluentAssertions;
using System.Linq;
using System.Collections.Generic;
using Electricity.Application.Common.Models;
using Electricity.Application.Common.Exceptions;
using Electricity.Application.Common.Models.Dtos;
using Electricity.Application.PowerFactor.Queries.GetPowerFactorOverview;

namespace Electricity.Application.IntegrationTests.PowerFactor.Queries
{
    using static Testing;

    public class GetPowerFactorOverviewTest : TestBase
    {
        [Test]
        public async Task ShouldRequireInterval1()
        {
            await RunAsDefaultTenantAndUser();

            var query = new GetPowerFactorOverviewQuery
            {
                Interval1 = null
            };

            FluentActions.Invoking(() =>
                SendAsync(query)).Should().Throw<ValidationException>();
        }

        [Test]
        public async Task ShouldReturnPowerFactorOverviewWhenUnboundedIntervalProvided()
        {
            await RunAsDefaultTenantAndUser();

            var query = new GetPowerFactorOverviewQuery
            {
                Interval1 = new IntervalDto(null, null)
            };

            var result = await SendAsync(query);

            result.Items1.Should().HaveCount(GetRecordGroupCount());

            foreach (var item in result.Items1)
            {
                item.GroupId.Should().NotBeNullOrWhiteSpace();
                item.GroupName.Should().NotBeNullOrWhiteSpace();

                item.ActiveEnergy.Should().BePositive();
                item.ReactiveEnergyL.Should().BePositive();
                item.ReactiveEnergyC.Should().BePositive();
                item.CosFi.Should().BePositive();
            }
        }

        [Test]
        public async Task ShouldReturnPowerFactorOverviewWhenInterval1Provided()
        {
            await RunAsDefaultTenantAndUser();

            var query = new GetPowerFactorOverviewQuery
            {
                Interval1 = new IntervalDto(new DateTime(2021, 1, 1), new DateTime(2021, 1, 10))
            };

            var result = await SendAsync(query);

            result.Items1.Should().HaveCount(GetRecordGroupCount());

            foreach (var item in result.Items1)
            {
                item.GroupId.Should().NotBeNullOrWhiteSpace();
                item.GroupName.Should().NotBeNullOrWhiteSpace();

                item.ActiveEnergy.Should().BePositive();
                item.ReactiveEnergyL.Should().BePositive();
                item.ReactiveEnergyC.Should().BePositive();
                item.CosFi.Should().BePositive();
            }
        }

        [Test]
        public async Task ShouldReturnPowerFactorOverviewWhen2IntervalsProvided()
        {
            await RunAsDefaultTenantAndUser();

            var query = new GetPowerFactorOverviewQuery
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

                item.ActiveEnergy.Should().BePositive();
                item.ReactiveEnergyL.Should().BePositive();
                item.ReactiveEnergyC.Should().BePositive();
                item.CosFi.Should().BePositive();
            }
        }
    }
}