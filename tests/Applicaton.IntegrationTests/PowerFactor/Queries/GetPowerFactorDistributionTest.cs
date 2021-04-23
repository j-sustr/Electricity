using System;
using System.Threading.Tasks;
using NUnit.Framework;
using FluentAssertions;
using Electricity.Application.Common.Exceptions;
using Electricity.Application.Common.Models.Dtos;
using Electricity.Application.PowerFactor.Queries.GetPowerFactorDistribution;
using Electricity.Application.Common.Models;

namespace Electricity.Application.IntegrationTests.PowerFactor.Queries
{
    using static Testing;

    public class GetPowerFactorDistributionTest : TestBase
    {
        [Test]
        public async Task ShouldRequireGroupId()
        {
            await RunAsDefaultTenantAndUser();

            var query = new GetPowerFactorDistributionQuery
            {
                Interval1 = null
            };

            FluentActions.Invoking(() =>
                SendAsync(query)).Should().Throw<ValidationException>();
        }

        [Test]
        public async Task ShouldRequirePhases()
        {
            await RunAsDefaultTenantAndUser();

            var query = new GetPowerFactorDistributionQuery
            {
                Interval1 = new IntervalDto(null, null)
            };

            FluentActions.Invoking(() =>
                SendAsync(query)).Should().Throw<ValidationException>();
        }

        [Test]
        public async Task ShouldReturnPowerFactorOverviewWhenUnboundedIntervalProvided()
        {
            await RunAsDefaultTenantAndUser();

            var query = new GetPowerFactorDistributionQuery
            {
                GroupId = GetRecordGroupIdByName("Mistnost101"),
                Interval1 = new IntervalDto(null, null),
                Phases = new Phases
                {
                    Main = true
                }
            };

            var result = await SendAsync(query);

            result.GroupName.Should().NotBeNullOrWhiteSpace();
            result.Distribution1.Should().HaveCount(8);

            foreach (var item in result.Distribution1)
            {
                item.RangeName.Should().NotBeNullOrWhiteSpace();
                item.Range.Should().NotBeNull();

                item.ValueMain.Should().BeGreaterOrEqualTo(0);
                item.ValueL1.Should().BeNull();
                item.ValueL2.Should().BeNull();
                item.ValueL2.Should().BeNull();
            }
        }

        [Test]
        public async Task ShouldReturnPowerFactorOverviewWhenInterval1Provided()
        {
            await RunAsDefaultTenantAndUser();

            var query = new GetPowerFactorDistributionQuery
            {
                GroupId = GetRecordGroupIdByName("Mistnost101"),
                Interval1 = new IntervalDto(new DateTime(2021, 1, 1), new DateTime(2021, 1, 10)),
                Phases = new Phases
                {
                    Main = true
                }
            };

            var result = await SendAsync(query);

            result.GroupName.Should().NotBeNullOrWhiteSpace();
            result.Distribution1.Should().HaveCount(8);

            foreach (var item in result.Distribution1)
            {
                item.RangeName.Should().NotBeNullOrWhiteSpace();
                item.Range.Should().NotBeNull();

                item.ValueMain.Should().BeGreaterOrEqualTo(0);
                item.ValueL1.Should().BeNull();
                item.ValueL2.Should().BeNull();
                item.ValueL2.Should().BeNull();
            }
        }

        [Test]
        public async Task ShouldReturnPowerFactorOverviewWhen2IntervalsProvided()
        {
            await RunAsDefaultTenantAndUser();

            var query = new GetPowerFactorDistributionQuery
            {
                GroupId = GetRecordGroupIdByName("Mistnost101"),
                Interval1 = new IntervalDto(new DateTime(2021, 1, 1), new DateTime(2021, 1, 10)),
                Interval2 = new IntervalDto(new DateTime(2021, 1, 10), new DateTime(2021, 1, 20)),
                Phases = new Phases
                {
                    L2 = true,
                    L3 = true
                }
            };

            var result = await SendAsync(query);

            result.Distribution1.Should().HaveCount(8);
            result.Distribution2.Should().HaveCount(8);

            foreach (var item in result.Distribution1)
            {
                item.RangeName.Should().NotBeNullOrWhiteSpace();
                item.Range.Should().NotBeNull();

                item.ValueMain.Should().BeNull();
                item.ValueL1.Should().BeNull();
                item.ValueL2.Should().BeGreaterOrEqualTo(0);
                item.ValueL3.Should().BeGreaterOrEqualTo(0);
            }

            foreach (var item in result.Distribution2)
            {
                item.RangeName.Should().NotBeNullOrWhiteSpace();
                item.Range.Should().NotBeNull();

                item.ValueMain.Should().BeNull();
                item.ValueL1.Should().BeNull();
                item.ValueL2.Should().BeGreaterOrEqualTo(0);
                item.ValueL3.Should().BeGreaterOrEqualTo(0);
            }
        }
    }
}