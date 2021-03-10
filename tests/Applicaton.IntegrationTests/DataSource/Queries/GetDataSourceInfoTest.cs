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

            result.MinDatetime.Should().Be(new DateTime(2021, 1, 1));
            result.MaxDatetime.Should().Be(new DateTime(2021, 2, 28, 23, 59, 50));
        }
    }
}