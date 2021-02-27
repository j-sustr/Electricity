using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electricity.Application.Costs.Queries.GetCostsOverview
{
    public class GetCostsOverviewQueryValidator : AbstractValidator<GetCostsOverviewQuery>
    {
        public GetCostsOverviewQueryValidator()
        {
            RuleFor(x => x.Interval1)
               .NotNull()
               .WithMessage("Interval1 is required.");
        }
    }
}