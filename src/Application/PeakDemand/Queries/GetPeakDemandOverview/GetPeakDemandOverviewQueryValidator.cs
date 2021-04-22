using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electricity.Application.PeakDemand.Queries.GetPeakDemandOverview
{
    public class GetPeakDemandOverviewQueryValidator : AbstractValidator<GetPeakDemandOverviewQuery>
    {
        public GetPeakDemandOverviewQueryValidator()
        {
            RuleFor(x => x.Interval1)
               .NotNull()
               .WithMessage("Interval1 is required.");
        }
    }
}