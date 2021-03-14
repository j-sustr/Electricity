using Electricity.Application.Common.Models.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Electricity.Application.PeakDemand.Queries.GetPeakDemandOverview
{
    public class GetPeakDemandOverviewQuery : IRequest<PeakDemandOverviewDto>
    {
        public IntervalDto Interval1 { get; set; }

        public IntervalDto? Interval2 { get; set; }
    }

    public class GetCostsDetailQueryHandler : IRequestHandler<GetPeakDemandOverviewQuery, PeakDemandOverviewDto>
    {
        public Task<PeakDemandOverviewDto> Handle(GetPeakDemandOverviewQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}