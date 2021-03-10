using Electricity.Application.DataSource.Queries.GetDataSourceInfo;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Electricity.Application.IntegrationTests.DataSource.Queries
{
    using static Testing;

    public class GetDataSourceInfoTest
    {
        [Test]
        public async Task ShouldReturnInfo()
        {
            var userId = await RunAsDefaultUserAsync();

            var query = new GetDataSourceInfoQuery
            {
                GroupId = null,
                Arch = 0,
            };

            var result = await SendAsync(query);

            var start = DateTime.SpecifyKind(new DateTime(2021, 1, 1), DateTimeKind.Local);
            var startU = start.ToUniversalTime();
            var start2 = new DateTime(2021, 1, 1);

            result.MinDatetime.Should().Be(new DateTime(2021, 1, 1));
            result.MaxDatetime.Should().Be(new DateTime(2021, 3, 1));
        }
    }
}