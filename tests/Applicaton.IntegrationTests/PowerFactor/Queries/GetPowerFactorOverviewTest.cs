using System;
using System.Threading.Tasks;
using Electricity.Application.PowerFactor.Queries.GetPowerFactorOverviewQuery;
using NUnit.Framework;
using FluentAssertions;
using System.Linq;
using System.Collections.Generic;
using Electricity.Application.Common.Models;
using Electricity.Application.Common.Exceptions;
using Electricity.Application.Common.Models.Dtos;

namespace Electricity.Application.IntegrationTests.PowerFactor.Queries
{
    using static Testing;

    public class GetPowerFactorOverviewTest
    {
        [Test]
        public async Task ShouldRequireInterval1()
        {
            var userId = await RunAsDefaultUserAsync();

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
            var userId = await RunAsDefaultUserAsync();

            var query = new GetPowerFactorOverviewQuery
            {
                Interval1 = new IntervalDto(null, null)
            };

            var result = await SendAsync(query);

            result.Data.Should().HaveCount(1);

            foreach (var data in result.Data)
            {
                data.Items.Should().HaveCount(3);

                foreach (var group in data.Items)
                {
                    group.DeviceName.Should().NotBeNullOrWhiteSpace();

                    group.ActiveEnergy.Should().BePositive();
                    group.ReactiveEnergyL.Should().BePositive();
                    group.ReactiveEnergyC.Should().BePositive();
                    group.CosFi.Should().BePositive();
                }
            }
        }

        [Test]
        public async Task ShouldReturnPowerFactorOverviewWhenInterval1Provided()
        {
            var userId = await RunAsDefaultUserAsync();

            var query = new GetPowerFactorOverviewQuery
            {
                Interval1 = new IntervalDto(new DateTime(2021, 1, 1), new DateTime(2021, 1, 10))
            };

            var result = await SendAsync(query);

            result.Data.Should().HaveCount(1);

            foreach (var data in result.Data)
            {
                data.Items.Should().HaveCount(3);

                foreach (var group in data.Items)
                {
                    group.DeviceName.Should().NotBeNullOrWhiteSpace();

                    group.ActiveEnergy.Should().BePositive();
                    group.ReactiveEnergyL.Should().BePositive();
                    group.ReactiveEnergyC.Should().BePositive();
                    group.CosFi.Should().BePositive();
                }
            }
        }

        [Test]
        public async Task ShouldReturnPowerFactorOverviewWhen2IntervalsProvided()
        {
            var userId = await RunAsDefaultUserAsync();

            var query = new GetPowerFactorOverviewQuery
            {
                Interval1 = new IntervalDto(new DateTime(2021, 1, 1), new DateTime(2021, 1, 10)),
                Interval2 = new IntervalDto(new DateTime(2021, 1, 10), new DateTime(2021, 1, 20))
            };

            var result = await SendAsync(query);

            result.Data.Should().HaveCount(2);

            foreach (var data in result.Data)
            {
                data.Items.Should().HaveCount(3);

                foreach (var group in data.Items)
                {
                    group.DeviceName.Should().NotBeNullOrWhiteSpace();

                    group.ActiveEnergy.Should().BePositive();
                    group.ReactiveEnergyL.Should().BePositive();
                    group.ReactiveEnergyC.Should().BePositive();
                    group.CosFi.Should().BePositive();
                }
            }
        }
    }
}