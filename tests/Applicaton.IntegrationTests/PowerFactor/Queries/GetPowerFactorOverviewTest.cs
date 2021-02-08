using System;
using System.Threading.Tasks;
using Electricity.Application.PowerFactor.Queries.GetPowerFactorOverviewQuery;
using NUnit.Framework;
using FluentAssertions;
using System.Linq;
using System.Collections.Generic;
using Electricity.Application.Common.Models;
using Electricity.Application.Common.Exceptions;

namespace Electricity.Application.IntegrationTests.PowerFactor.Queries
{
    using static Testing;

    public class GetPowerFactorOverviewTest
    {
        [Test]
        public async Task ShouldRequireIntervals()
        {
            var userId = await RunAsDefaultUserAsync();

            var query = new GetPowerFactorOverviewQuery();

            FluentActions.Invoking(() =>
                SendAsync(query)).Should().Throw<ValidationException>();
        }

        [Test]
        public async Task ShouldRequireIntervalsToNotBeEmpty()
        {
            var userId = await RunAsDefaultUserAsync();

            var query = new GetPowerFactorOverviewQuery
            {
                Intervals = new Interval[] { }
            };

            FluentActions.Invoking(() =>
                SendAsync(query)).Should().Throw<ValidationException>();
        }

        [Test]
        public async Task ShouldReturnPowerFactorOverviewWhenNullIntervalProvided()
        {
            var userId = await RunAsDefaultUserAsync();

            var query = new GetPowerFactorOverviewQuery
            {
                Intervals = new Interval[] { null }
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
                    group.TanFi.Should().BePositive();
                }
            }
        }

        [Test]
        public async Task ShouldReturnPowerFactorOverviewWhen1IntervalProvided()
        {
            var userId = await RunAsDefaultUserAsync();

            var query = new GetPowerFactorOverviewQuery
            {
                Intervals = new Interval[]
                {
                    new Interval(new DateTime(2021, 1, 1), new DateTime(2021, 1, 10))
                }
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
                    group.TanFi.Should().BePositive();
                }
            }
        }

        [Test]
        public async Task ShouldReturnPowerFactorOverviewFor2Intervals()
        {
            var userId = await RunAsDefaultUserAsync();

            var query = new GetPowerFactorOverviewQuery
            {
                Intervals = new Interval[]
                {
                    new Interval(new DateTime(2021, 1, 1), new DateTime(2021, 1, 10)),
                    new Interval(new DateTime(2021, 1, 10), new DateTime(2021, 1, 20))
                }
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
                    group.TanFi.Should().BePositive();
                }
            }
        }
    }
}