using Electricity.Application.Common.Exceptions;
using Electricity.Application.Common.Models.Dtos;
using Electricity.Application.PeakDemand.Queries.GetPeakDemandDetail;
using FluentAssertions;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Electricity.Application.IntegrationTests.PeakDemand.Queries
{
    using static Testing;
    class GetPeakDemandDetailTest : TestBase
    {
        [Test]
        public async Task ShouldRequireGroupId()
        {
            await RunAsDefaultTenantAndUser();

            var query = new GetPeakDemandDetailQuery
            {
                GroupId = GetRecordGroupIdByName("Mistnost101")
            };

            FluentActions.Invoking(() =>
                SendAsync(query)).Should().Throw<ValidationException>();
        }


        [Test]
        public async Task ShouldReturnPeakDemandDetailWhenUnboundedIntervalProvided()
        {
            await RunAsDefaultTenantAndUser();

            var query = new GetPeakDemandDetailQuery
            {
                GroupId = GetRecordGroupIdByName("Mistnost101"),
                Interval1 = new IntervalDto(null, null)
            };

            var result = await SendAsync(query);

            result.Data1.Should().NotBeNull();
            result.Data1?.DemandSeries.Should().NotBeEmpty();
        }
    }
}
