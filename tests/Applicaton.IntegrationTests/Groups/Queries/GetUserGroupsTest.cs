using Electricity.Application.Groups.Queries.GetUserGroupInfoTree;
using FluentAssertions;
using NUnit.Framework;
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

            var query = new GetUserGroupInfoTreeQuery();

            var result = await SendAsync(query);

            var tree = GetGroupTree();

            result.Subgroups.Should().HaveCount(tree.Subgroups.Count);

            foreach (var g in result.Subgroups)
            {
                g.ID.Should().NotBeEmpty();
                g.Name.Should().NotBeNullOrWhiteSpace();
            }
        }
    }
}