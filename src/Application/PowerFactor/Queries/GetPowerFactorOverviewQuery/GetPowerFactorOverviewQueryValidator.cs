using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Electricity.Application.PowerFactor.Queries.GetPowerFactorOverviewQuery
{
    public class GetPowerFactorOverviewQueryValidator : AbstractValidator<GetPowerFactorOverviewQuery>
    {
        public GetPowerFactorOverviewQueryValidator()
        {
            RuleFor(x => x.Interval1)
               .NotNull()
               .WithMessage("Interval1 is required.");
        }
    }
}