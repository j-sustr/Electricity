using Electricity.Application.Groups.Queries.GetUserGroups;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Electricity.Application.IntegrationTests.Groups.Queries
{
    using static Testing;

    public class GetUserGroupsTest : TestBase
    {
        [Test]
        public async Task ShouldReturnUserGroups()
        {
            await RunAsDefaultTenantAndUser();

            var query = new GetUserGroupsQuery();

            var result = await SendAsync(query);

            result.Groups.Should().HaveCount(GetUserGroupCount());

            foreach (var g in result.Groups)
            {
                g.Id.Should().NotBeEmpty();
                g.Name.Should().NotBeNullOrWhiteSpace();
            }
        }
    }
}