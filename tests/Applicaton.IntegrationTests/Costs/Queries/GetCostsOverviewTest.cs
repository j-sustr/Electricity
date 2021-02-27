﻿using Electricity.Application.Common.Models.Dtos;
using Electricity.Application.Costs.Queries.GetCostsOverview;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Electricity.Application.IntegrationTests.Costs.Queries
{
    using static Testing;

    public class GetCostsOverviewTest
    {
        [Test]
        public async Task ShouldReturnCostsOverviewWhenInfiniteIntervalProvided()
        {
            var userId = await RunAsDefaultUserAsync();

            var query = new GetCostsOverviewQuery
            {
                Interval1 = new IntervalDto(null, null)
            };

            var result = await SendAsync(query);

            result.Items1.Should().HaveCount(3);
            result.Items2.Should().BeNull();

            foreach (var item in result.Items1)
            {
                item.GroupName.Should().NotBeNullOrWhiteSpace();

                item.ActiveEnergyInMonths.Should().OnlyContain(x => x > 0);
                item.ReactiveEnergyInMonths.Should().OnlyContain(x => x > 0);
                item.PeakDemandInMonths.Should().OnlyContain(x => x > 0);
            }
        }
    }
}